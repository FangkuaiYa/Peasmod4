using Hazel;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;

namespace Peasmod4.API.Networking;

[RegisterCustomRpc((uint)CustomRpcCalls.EndGame)]
public class RpcEndGame : PlayerCustomRpc<PeasmodPlugin, RpcEndGame.Data>
{
    public RpcEndGame(PeasmodPlugin plugin, uint id) : base(plugin, id)
    {
    }

    public override RpcLocalHandling LocalHandling => RpcLocalHandling.After;

    public override void Write(MessageWriter writer, Data data)
    {
        writer.Write((byte)data.EndReason);
    }

    public override Data Read(MessageReader reader)
    {
        var endReason = (GameOverReason)reader.ReadByte();
        PeasmodPlugin.Logger.LogInfo("Received RpcEndGame : " + endReason);
        return new Data(endReason);
    }

    public override void Handle(PlayerControl innerNetObject, Data data)
    {
        if (AmongUsClient.Instance.AmHost)
            //GameManager.Instance.RpcEndGame(data.EndReason, false);
            Utility.DoAfterTimeout(() => GameManager.Instance.RpcEndGame(data.EndReason, false), 0.5f);
    }

    public readonly struct Data
    {
        public readonly GameOverReason EndReason;

        public Data(GameOverReason endReason)
        {
            EndReason = endReason;
        }
    }
}