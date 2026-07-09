using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Managers;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Doc.DocCode.CardPools;
using Doc.DocCode.Cards.Doctor;
using Doc.DocCode.Cards.Doctor.Basic;
using Doc.DocCode.Relics;
using Doc.DocCode.Cards;

namespace Doc;

[ModInitializer(nameof(Initialize))]
public class MainFile
{
    public const string ModId = "Doc";

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        AddCardsToPools();
        AddRelicsToPools();
    }

    private static void AddCardsToPools()
    {
        
        ModHelper.AddModelToPool<DocCardPool, DocAttack>();
        ModHelper.AddModelToPool<DocCardPool, DocDefence>();
        ModHelper.AddModelToPool<DocCardPool, Orchestrate>();
        ModHelper.AddModelToPool<DocCardPool, Plan>();
        ModHelper.AddModelToPool<DocCardPool, Gravel>();
        ModHelper.AddModelToPool<DocCardPool, Nearl>();
        ModHelper.AddModelToPool<DocCardPool, Mlynar>();
        ModHelper.AddModelToPool<DocCardPool, Ashlock>();
        ModHelper.AddModelToPool<DocCardPool, Flametail>();
        ModHelper.AddModelToPool<DocCardPool, WildMane>();
        ModHelper.AddModelToPool<DocCardPool, Platinum>();
        ModHelper.AddModelToPool<DocCardPool, Whislash>();
        ModHelper.AddModelToPool<DocCardPool, Meteor>();
        ModHelper.AddModelToPool<DocCardPool, Proviso>();
        ModHelper.AddModelToPool<DocCardPool, JusticeKnight>();
        ModHelper.AddModelToPool<DocCardPool, Blemishine>();
        ModHelper.AddModelToPool<DocCardPool, NearlTheRadianKnight>();
        ModHelper.AddModelToPool<DocCardPool, Beeswax>();

        ModHelper.AddModelToPool<DocCardPool, Estelle>();
        ModHelper.AddModelToPool<DocCardPool, Bubble>();
        ModHelper.AddModelToPool<DocCardPool, Papyrus>();
        ModHelper.AddModelToPool<DocCardPool, Philae>();
        ModHelper.AddModelToPool<DocCardPool, Minimalist>();
        ModHelper.AddModelToPool<DocCardPool, Gavial>();
        ModHelper.AddModelToPool<DocCardPool, Titi>();
        ModHelper.AddModelToPool<DocCardPool, Eunectes>();



        //衍生
        ModHelper.AddModelToPool<TokenCardPool, UndeclaredRage>();
        ModHelper.AddModelToPool<TokenCardPool, UnexoneratedSorrow>();
        ModHelper.AddModelToPool<TokenCardPool, UngloriousGlory>();

        //普通无色
        ModHelper.AddModelToPool<ColorlessCardPool, KnightOath>();
    }

    private static void AddRelicsToPools()
    {
        // 银印添加到共享池（用于先古之民事件）
        ModHelper.AddModelToPool<SharedRelicPool, DoctorSilverSeal>();
    }
}