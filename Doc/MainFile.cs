using BaseLib.Utils;
using Doc.DocCode.CardPools;
using Doc.DocCode.Cards;
using Doc.DocCode.Cards.Doctor;
using Doc.DocCode.Cards.Doctor.Basic;
using Doc.DocCode.RelicsPools;
using Doc.DocCode.Relics;
using Doc.DocCode.RelicsPools;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace Doc;

[ModInitializer(nameof(Initialize))]
public class MainFile
{
    public const string ModId = "Doc";

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        // 1. 先扫描所有标记了 [Pool] 的类型（这是关键！）
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(MainFile).Assembly);

        // 2. 应用 Harmony 补丁
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // 3. 添加卡牌到卡池
        AddCardsToPools();

        // 4. 添加遗物到遗物池
        AddRelicsToPools();

        Logger.Info("Doc mod initialized.");
    }

    private static void AddCardsToPools()
    {
        ModHelper.AddModelToPool<DocCardPool, DocAttack>();
        ModHelper.AddModelToPool<DocCardPool, DocDefence>();
        ModHelper.AddModelToPool<DocCardPool, Orchestrate>();
        ModHelper.AddModelToPool<DocCardPool, Plan>();
        ModHelper.AddModelToPool<DocCardPool, MacroStrategy>();
        ModHelper.AddModelToPool<DocCardPool, LessonLearned>();

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
        ModHelper.AddModelToPool<DocCardPool, GavialTheInvincible>();
        ModHelper.AddModelToPool<DocCardPool, Carnelian>();
        ModHelper.AddModelToPool<DocCardPool, Lancet_2>();
        ModHelper.AddModelToPool<DocCardPool, SandReckoner>();
        ModHelper.AddModelToPool<DocCardPool, Scene>();
        ModHelper.AddModelToPool<DocCardPool, Wulfenite>();
        ModHelper.AddModelToPool<DocCardPool, Kestrel>();
        ModHelper.AddModelToPool<DocCardPool, Hadiya>();
        ModHelper.AddModelToPool<DocCardPool, Manticore>();
        ModHelper.AddModelToPool<DocCardPool, Pepe>();
        ModHelper.AddModelToPool<DocCardPool, Sesa>();
        ModHelper.AddModelToPool<DocCardPool, Narantuya>();
        ModHelper.AddModelToPool<DocCardPool, Passenger>();
        ModHelper.AddModelToPool<DocCardPool, Tuye>();

        // 衍生卡
        ModHelper.AddModelToPool<TokenCardPool, UndeclaredRage>();
        ModHelper.AddModelToPool<TokenCardPool, UnexoneratedSorrow>();
        ModHelper.AddModelToPool<TokenCardPool, UngloriousGlory>();
        ModHelper.AddModelToPool<TokenCardPool, Recon>();
        ModHelper.AddModelToPool<TokenCardPool, RockyChomper>();
        ModHelper.AddModelToPool<TokenCardPool, HadiyaII>();

        // 普通无色
        ModHelper.AddModelToPool<ColorlessCardPool, KnightOath>();
    }

    private static void AddRelicsToPools()
    {
        // 铜印添加到遗物池
        ModHelper.AddModelToPool<DoctorRelicPool, HrBronzeSeal>();

        ModHelper.AddModelToPool<EventRelicPool, DoctorSilverSeal>();
    }
}