﻿using ServerCore;
using Server;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        C_Move movePacket = packet as C_Move;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession, movePacket));
    }
}
