﻿using DarkRift;
using DarkRift.Server;

public static class DarkriftExtensions
{
	public static void SendMessage<T>(this IClient client, T message, ushort tag, SendMode sendMode) where T : IDarkRiftSerializable
	{
		using (DarkRiftWriter writer = DarkRiftWriter.Create())
		{
			writer.Write(message);
			using (Message msg = Message.Create(tag, writer)) 
			{
				client.SendMessage(msg, sendMode);
			}
		}
	}

	public static void SendMessage(this IClient client, string message, ushort tag, SendMode sendMode)
	{
		using (DarkRiftWriter writer = DarkRiftWriter.Create())
		{
			writer.Write(message);
			using (Message msg = Message.Create(tag, writer)) 
			{
				client.SendMessage(msg, sendMode);
			}
		}
	}
	public static void SendMessage(this IClient client, ushort message, ushort tag, SendMode sendMode)
	{
		using (DarkRiftWriter writer = DarkRiftWriter.Create())
		{
			writer.Write(message);
			using (Message msg = Message.Create(tag, writer)) 
			{
				client.SendMessage(msg, sendMode);
			}
		}
	}
	public static void SendMessageNoContent(this IClient client, ushort tag, SendMode sendMode)
	{
		using (DarkRiftWriter writer = DarkRiftWriter.Create())
		{
			using (Message msg = Message.Create(tag, writer)) 
			{
				client.SendMessage(msg, sendMode);
			}
		}
	}

}
