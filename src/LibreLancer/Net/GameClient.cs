﻿/* The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * 
 * The Initial Developer of the Original Code is Callum McGing (mailto:callum.mcging@gmail.com).
 * Portions created by the Initial Developer are Copyright (C) 2013-2017
 * the Initial Developer. All Rights Reserved.
 */
using System;
using System.Net;
using System.Threading;
using Lidgren.Network;
namespace LibreLancer
{
	public class GameClient : IDisposable
	{
		bool running = false;

		IUIThread mainThread;
		Thread networkThread;
		NetClient client;
		public event Action<LocalServerInfo> ServerFound;
		public event Action<CharacterSelectInfo> CharacterSelection;
		public event Action<string> Disconnected;
		public event Action<string> AuthenticationRequired;

		public Guid UUID;

		public GameClient(IUIThread mainThread)
		{
			this.mainThread = mainThread;
		}

		public void Start()
		{
			running = true;
			networkThread = new Thread(NetworkThread);
            networkThread.Name = "NetClient";
            networkThread.Start();
		}

		public void Stop()
		{
			running = false;
			networkThread.Join();
		}

		public void Dispose()
		{
			if (running) Stop();
		}

		public void DiscoverLocalPeers()
		{
			if (running)
			{
				while (client == null) Thread.Sleep(0);
				client.DiscoverLocalPeers(NetConstants.DEFAULT_PORT);
			}
		}

		public void DiscoverGlobalPeers()
		{
			//HTTP?
		}

		bool connecting = true;
		public void Connect(IPEndPoint endPoint)
		{
			connecting = true;
			var message = client.CreateMessage();
			message.Write("Hello World!");
			client.Connect(endPoint, message);
		}

		public void Authenticate(string token)
		{
			var response = client.CreateMessage();
			response.Write((byte)PacketKind.Authentication);
			response.Write((byte)AuthenticationKind.Token);
			response.Write(token);
			client.SendMessage(response, NetDeliveryMethod.ReliableOrdered);
		}

		public void RequestCharacterCreate()
		{
			var om = client.CreateMessage();
			om.Write((byte)PacketKind.NewCharacter);
			client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
		}

		void NetworkThread()
		{
			var conf = new NetPeerConfiguration(NetConstants.DEFAULT_APP_IDENT);
			client = new NetClient(conf);
			client.Start();
			NetIncomingMessage im;
			while (running)
			{
				while ((im = client.ReadMessage()) != null)
				{
					try
					{
						switch (im.MessageType)
						{
							case NetIncomingMessageType.DebugMessage:
							case NetIncomingMessageType.ErrorMessage:
							case NetIncomingMessageType.WarningMessage:
							case NetIncomingMessageType.VerboseDebugMessage:
								FLLog.Info("Lidgren", im.ReadString());
								break;
							case NetIncomingMessageType.DiscoveryResponse:
								if (ServerFound != null)
								{
									var info = new LocalServerInfo();
									info.EndPoint = im.SenderEndPoint;
									info.Name = im.ReadString();
									info.Description = im.ReadString();
									info.CurrentPlayers = im.ReadInt32();
									info.MaxPlayers = im.ReadInt32();
									mainThread.QueueUIThread(() => ServerFound(info));
								}
								break;
							case NetIncomingMessageType.StatusChanged:
								var status = (NetConnectionStatus)im.ReadByte();
								if (status == NetConnectionStatus.Disconnected)
								{
									connecting = false;
									Disconnected(im.ReadString());
								}
								break;
							case NetIncomingMessageType.Data:
								var kind = (PacketKind)im.ReadByte();
								if (connecting)
								{
									if (kind == PacketKind.Authentication)
									{
										FLLog.Info("Net", "Authentication Packet Received");
										var authkind = (AuthenticationKind)im.ReadByte();
										if (authkind == AuthenticationKind.Token)
										{
											FLLog.Info("Net", "Token");
											AuthenticationRequired(im.ReadString());
										}
										else if (authkind == AuthenticationKind.GUID)
										{
											FLLog.Info("Net", "GUID");
											var response = client.CreateMessage();
											response.Write((byte)PacketKind.Authentication);
											response.Write((byte)AuthenticationKind.GUID);
											var arr = UUID.ToByteArray();
											foreach (var b in arr) response.Write(b);
											client.SendMessage(response, NetDeliveryMethod.ReliableOrdered);
										}
										else
										{
											client.Shutdown("Invalid Packet");
										}
									}
									else if (kind == PacketKind.AuthenticationSuccess)
									{
										connecting = false;
										var inf = new CharacterSelectInfo();
										inf.ServerNews = im.ReadString();
										mainThread.QueueUIThread(() => CharacterSelection(inf));
									}
									else
									{
										client.Shutdown("Invalid Packet");
									}
									break;
								}
								switch (kind)
								{
									case PacketKind.NewCharacter:
										
										break;
								}
								break;
						}
					}
					catch (Exception)
					{
						FLLog.Error("Net", "Error reading message of type " + im.MessageType.ToString());
					}
					client.Recycle(im);
				}
				Thread.Sleep(1);
			}
			FLLog.Info("Lidgren", "Client shutdown");
			client.Shutdown("Shutdown");
		}
	}
}
