﻿using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace Twitch7
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, Misc, KillStealMenu, Items;
        public static Item Botrk;
        public static Item Bil;
        public static Item Youmuu;
        public static readonly Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static readonly Item Simitar = new Item(ItemId.Mercurial_Scimitar);
        public static readonly int[] SDamage = {0, 15, 20, 25, 30, 35};
        public static readonly int[] BDamage = {0, 20, 35, 50, 65, 80};
        public const float YOff = 10;
        public const float XOff = 0;
        public const float Width = 107;
        public const float Thick = 9;
        public static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }
        private static float HealthPercent()
        {
            return (PlayerInstance.Health / PlayerInstance.MaxHealth) * 100;
        }
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell.Active Q;
        public static Spell.Skillshot W;
        public static Spell.Active E;
        public static Spell.Active R;
        public static Spell.Targeted Ignite;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        static void OnLoadingComplete(EventArgs args)
        {
            if (!_Player.ChampionName.Contains("Twitch")) return;
            Chat.Print("Twitch7 Loaded!", Color.Orange);
            Bootstrap.Init(null);
            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 250, 1550, 275);
            W.AllowedCollisionCount = int.MaxValue;
            E = new Spell.Active(SpellSlot.E, 1200);
            R = new Spell.Active(SpellSlot.R);
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
            Bil = new Item(3144, 475f);
            Youmuu = new Item(3142, 10);
            Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
            Menu = MainMenu.AddMenu("Twitch7", "Twitch");
            Menu.AddSeparator();
            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("ComboQ", new CheckBox("Spell [Q]"));
            ComboMenu.Add("ComboW", new CheckBox("Spell [W]"));
            ComboMenu.AddGroupLabel("Combo [E] Settings");
            ComboMenu.Add("ComboE", new CheckBox("Spell [E]"));
            ComboMenu.Add("MinEC", new Slider("Min Stacks Use [E]", 5, 0, 6));
            ComboMenu.AddGroupLabel("Combo [E] On");
            foreach (var target in EntityManager.Heroes.Enemies)
            {
                ComboMenu.Add("combo" + target.ChampionName, new CheckBox("" + target.ChampionName));
            }
            ComboMenu.AddSeparator();
            ComboMenu.Add("ComboR", new CheckBox("Spell [R]"));
            ComboMenu.Add("MinR", new Slider("Min Enemies Use [R]", 3, 0, 5));

            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("HarassW", new CheckBox("Use [W]", false));
            HarassMenu.Add("HarassQ", new CheckBox("Use [Q]", false));
            HarassMenu.Add("HminQ", new Slider("Min Enemies Use [Q]", 2, 0, 5));
            HarassMenu.AddGroupLabel("Harass [E] Settings");
            HarassMenu.Add("HarassE", new CheckBox("Use [E]"));
            HarassMenu.Add("HminE", new Slider("Min Stacks Use [E]", 5, 0, 6));
            HarassMenu.AddGroupLabel("Harass [E] On");
            foreach (var target in EntityManager.Heroes.Enemies)
            {
                HarassMenu.Add("haras" + target.ChampionName, new CheckBox("" + target.ChampionName));
            }
            HarassMenu.Add("ManaQ", new Slider("Min Mana For Harass", 40));

            LaneClearMenu = Menu.AddSubMenu("LaneClear Settings", "LaneClear");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.Add("ELH", new CheckBox("Only Use [E] LastHit", false));
            LaneClearMenu.Add("WLC", new CheckBox("Use [W] LaneClear", false));
            LaneClearMenu.Add("ManaLC", new Slider("Min Mana For LaneClear", 40));

            JungleClearMenu = Menu.AddSubMenu("JungleClear Settings", "JungleClear");
            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            JungleClearMenu.Add("WJungle", new CheckBox("Use [W] JungleClear"));
            JungleClearMenu.Add("MnJungle", new Slider("Min Mana For JungleClear", 30));
            JungleClearMenu.AddGroupLabel("[E] Settings");
            JungleClearMenu.Add("EDragon", new CheckBox("Use [E] Ks"));
            JungleClearMenu.AddSeparator();
            JungleClearMenu.Add("jungleSRU_Baron", new CheckBox("Baron"));
            JungleClearMenu.Add("jungleSRU_Dragon_Elder", new CheckBox("Elder Dragon"));
            JungleClearMenu.Add("jungleSRU_Dragon_Air", new CheckBox("Air Dragon"));
            JungleClearMenu.Add("jungleSRU_Dragon_Earth", new CheckBox("Fire Dragon"));
            JungleClearMenu.Add("jungleSRU_Dragon_Fire", new CheckBox("Earth Dragon"));
            JungleClearMenu.Add("jungleSRU_Dragon_Water", new CheckBox("Water Dragon"));
            JungleClearMenu.Add("jungleSRU_Red", new CheckBox("Red"));
            JungleClearMenu.Add("jungleSRU_Blue", new CheckBox("Blue"));

            KillStealMenu = Menu.AddSubMenu("KillSteal Settings", "KillSteal");
            KillStealMenu.AddGroupLabel("KillSteal Settings");
            KillStealMenu.Add("KsW", new CheckBox("Use [W] KillSteal"));
            KillStealMenu.Add("KsE", new CheckBox("Use [E] KillSteal"));
            KillStealMenu.Add("ign", new CheckBox("Use [Ignite] KillSteal"));

            Misc = Menu.AddSubMenu("Misc Settings", "Misc");
            Misc.AddGroupLabel("Misc Settings");
            Misc.Add("AntiGap", new CheckBox("Use [W] AntiGapcloser"));
            Misc.Add("FleeQ", new CheckBox("Use [Q] Flee"));
            Misc.Add("FleeW", new CheckBox("Use [W] Flee"));
            Misc.AddGroupLabel("Use [E] Enemy Out Range");
            Misc.Add("ERanh", new CheckBox("Use [E] If Enemy Escape", false));
            Misc.Add("ERanhs", new Slider("Min Stacks Use [E]", 6, 1, 6));
            Misc.AddGroupLabel("Skin Changer");
            Misc.Add("checkSkin", new CheckBox("Use Skin Changer", false));
            Misc.Add("skin.Id", new ComboBox("Skin Mode", 7, "Default", "1", "2", "3", "4", "5", "6", "7"));
            Misc.AddGroupLabel("Draw Settings");
            Misc.Add("DrawW", new CheckBox("[W] Range"));
            Misc.Add("DrawE", new CheckBox("[E] Range"));
            Misc.Add("Damage", new CheckBox("Damage Indicator"));

            Items = Menu.AddSubMenu("Items Settings", "Items");
            Items.AddGroupLabel("Items Settings");
            Items.Add("you", new CheckBox("Use [Youmuu]"));
            Items.Add("BOTRK", new CheckBox("Use [Botrk]"));
            Items.Add("ihp", new Slider("My HP Use BOTRK <=", 50));
            Items.Add("ihpp", new Slider("Enemy HP Use BOTRK <=", 50));
            Items.AddGroupLabel("Qss Settings");
            Items.Add("Qss", new CheckBox("Use Qss"));
            Items.AddGroupLabel("Qss On CC");
            Items.Add("stun", new CheckBox("Stuns"));
            Items.Add("rot", new CheckBox("Root"));
            Items.Add("tunt", new CheckBox("Taunt"));
            Items.Add("snare", new CheckBox("Snare"));
            Items.Add("charm", new CheckBox("Charm", false));
            Items.Add("slow", new CheckBox("Slows", false));
            Items.Add("blind", new CheckBox("Blinds", false));
            Items.Add("fear", new CheckBox("Fear", false));
            Items.Add("silence", new CheckBox("Silence", false));
            Items.Add("supperss", new CheckBox("Supperss", false));
            Items.Add("poly", new CheckBox("Polymorph", false));
            Items.Add("delay", new Slider("Humanizer Qss Delay", 0, 0, 1500));

            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Damage;
            Game.OnTick += Game_OnTick;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Misc["DrawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Orange, BorderWidth = 1, Radius = W.Range }.Draw(_Player.Position);
            }
            if (Misc["DrawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Orange, BorderWidth = 1, Radius = E.Range }.Draw(_Player.Position);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
            KillSteal();
            Item();
            Qsss();
            if (_Player.SkinId != Misc["skin.Id"].Cast<ComboBox>().CurrentValue)
            {
                if (checkSkin())
                {
                    Player.SetSkinId(SkinId());
                }
            }
        }

        public static int SkinId()
        {
            return Misc["skin.Id"].Cast<ComboBox>().CurrentValue;
        }
        public static bool checkSkin()
        {
            return Misc["checkSkin"].Cast<CheckBox>().CurrentValue;
        }

        public static void CastQss()
        {
            if (Qss.IsOwned() && Qss.IsReady())
            {
                Core.DelayAction(() => Qss.Cast(), Items["delay"].Cast<Slider>().CurrentValue);
            }

            if (Simitar.IsOwned() && Simitar.IsReady())
            {
                Core.DelayAction(() => Simitar.Cast(), Items["delay"].Cast<Slider>().CurrentValue);
            }
        }

        private static void Qsss()
        {
            if (!Items["Qss"].Cast<CheckBox>().CurrentValue) return;
            if (Items["snare"].Cast<CheckBox>().CurrentValue && Player.HasBuffOfType(BuffType.Snare))
            {
                CastQss();
            }
            if (Items["tunt"].Cast<CheckBox>().CurrentValue && Player.HasBuffOfType(BuffType.Taunt))
            {
                CastQss();
            }
            if (Items["stun"].Cast<CheckBox>().CurrentValue && Player.HasBuffOfType(BuffType.Stun))
            {
                CastQss();
            }
            if (Items["poly"].Cast<CheckBox>().CurrentValue && Player.HasBuffOfType(BuffType.Polymorph))
            {
                CastQss();
            }
            if (Items["blind"].Cast<CheckBox>().CurrentValue && Player.HasBuffOfType(BuffType.Blind))
            {
                CastQss();
            }
            if (Items["fear"].Cast<CheckBox>().CurrentValue && Player.HasBuffOfType(BuffType.Fear))
            {
                CastQss();
            }
            if (Items["charm"].Cast<CheckBox>().CurrentValue && Player.HasBuffOfType(BuffType.Charm))
            {
                CastQss();
            }
            if (Items["supperss"].Cast<CheckBox>().CurrentValue && Player.HasBuffOfType(BuffType.Suppression))
            {
                CastQss();
            }
            if (Items["silence"].Cast<CheckBox>().CurrentValue && Player.HasBuffOfType(BuffType.Silence))
            {
                CastQss();
            }
            if (Items["rot"].Cast<CheckBox>().CurrentValue && _Player.IsRooted)
            {
                CastQss();
            }
            if (Items["slow"].Cast<CheckBox>().CurrentValue && Player.HasBuffOfType(BuffType.Slow))
            {
                CastQss();
            }
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var useQ = ComboMenu["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["ComboW"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["ComboE"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["ComboR"].Cast<CheckBox>().CurrentValue;
            var MinR = ComboMenu["MinR"].Cast<Slider>().CurrentValue;
            var MinE = ComboMenu["MinEC"].Cast<Slider>().CurrentValue;
            if (target != null)
            {
                if (useQ && Q.IsReady() && W.IsInRange(target))
                {
                    Q.Cast();
                }
                if (useW && W.IsReady() && W.IsInRange(target))
                {
                    var Wpred = W.GetPrediction(target);
                    if (Wpred.HitChance >= HitChance.Medium)
                    {
                        W.Cast(Wpred.CastPosition);
                    }
                }
                if (useE && E.IsReady() && E.IsInRange(target) && target.HasBuff("twitchdeadlyvenom"))
                {
                    if (ComboMenu["combo" + target.ChampionName].Cast<CheckBox>().CurrentValue && Stack(target) >= MinE)
                    {
                        E.Cast();
                    }
                }
                if (useR && R.IsReady() && _Player.Position.CountEnemiesInRange(W.Range) >= MinR)
                {
                    R.Cast();
                }
            }
        }

        public static void JungleClear()
        {
            var useW = JungleClearMenu["WJungle"].Cast<CheckBox>().CurrentValue;
            var mana = JungleClearMenu["MnJungle"].Cast<Slider>().CurrentValue;
            var Edra = JungleClearMenu["EDragon"].Cast<CheckBox>().CurrentValue;
            var monsters = EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.Position, W.Range).OrderByDescending(a => a.MaxHealth).FirstOrDefault();
            var baby = EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.Position, E.Range).Where(b => E.IsInRange(b) && b.Health < EDamage(b) && babi.Contains(b.BaseSkinName));
            if (monsters != null)
            {
                if (useW && W.IsReady() && W.IsInRange(monsters) && Player.Instance.ManaPercent >= mana)
                {
                    W.Cast(monsters);
                }
            }
            foreach (var m in baby)
            {
                if (Edra && JungleClearMenu["jungle" + m.BaseSkinName].Cast<CheckBox>().CurrentValue)
                {
                    E.Cast();
                }
            }
        }

        public static string[] babi =
        {
            "SRU_Blue", "SRU_Red", "SRU_Baron", "SRU_Dragon_Elder", "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water"
        };

        public static void Item()
        {

            var item = Items["BOTRK"].Cast<CheckBox>().CurrentValue;
            var yous = Items["you"].Cast<CheckBox>().CurrentValue;
            var Minhp = Items["ihp"].Cast<Slider>().CurrentValue;
            var Minhpp = Items["ihpp"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (target != null)
            {
                if (item && Bil.IsReady() && Bil.IsOwned() && target.IsValidTarget(550))
                {
                    Bil.Cast(target);
                }
                if ((item && Botrk.IsReady() && Botrk.IsOwned() && target.IsValidTarget(550)) && (Player.Instance.HealthPercent <= Minhp || target.HealthPercent < Minhpp))
                {
                    Botrk.Cast(target);
                }
                if (yous && Youmuu.IsReady() && Youmuu.IsOwned() && target.IsValidTarget(700) && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Youmuu.Cast();
                }
            }
        }

        private static void Flee()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var useQ = Misc["FleeQ"].Cast<CheckBox>().CurrentValue;
            var useW = Misc["FleeW"].Cast<CheckBox>().CurrentValue;
            if (useQ && Q.IsReady())
            {
                Q.Cast();
            }
            if (target != null)
            {
                if (useW && W.IsReady() && W.IsInRange(target))
                {
                    W.Cast(target);
                }
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (Misc["AntiGap"].Cast<CheckBox>().CurrentValue && sender.IsEnemy && e.Sender.Distance(_Player) < 300)
            {
                W.Cast(e.Sender);
            }
        }

        public static void LaneClear()
        {
            var mana = LaneClearMenu["ManaLC"].Cast<Slider>().CurrentValue;
            var useW = LaneClearMenu["WLC"].Cast<CheckBox>().CurrentValue;
            var lhE = LaneClearMenu["ELH"].Cast<CheckBox>().CurrentValue;
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions().Where(m => m.IsValidTarget(W.Range))
                .FirstOrDefault(unit => EntityManager.MinionsAndMonsters.EnemyMinions.Count(m => m.Distance(unit) < W.Radius) > 2);
            if (Player.Instance.ManaPercent < mana) return;
            if (minion != null)
            {
                if (lhE && minion.Health < EDamage(minion) && minion.HasBuff("twitchdeadlyvenom"))
                {
                    E.Cast();
                }
                if (useW && W.IsReady() && W.IsInRange(minion) && Player.Instance.ManaPercent >= mana)
                {
                    W.Cast(minion);
                }
            }
        }

        public static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].Cast<CheckBox>().CurrentValue;
            var useW = HarassMenu["HarassW"].Cast<CheckBox>().CurrentValue;
            var ManaQ = HarassMenu["ManaQ"].Cast<Slider>().CurrentValue;
            var MinQ = HarassMenu["HminQ"].Cast<Slider>().CurrentValue;
            var useE = HarassMenu["HarassE"].Cast<CheckBox>().CurrentValue;
            var MinE = HarassMenu["HminE"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (Player.Instance.ManaPercent <= ManaQ)
            {
                return;
            }
            if (target != null)
            {
                if (useQ && Q.IsReady() && E.IsInRange(target))
                {
                    if (_Player.Position.CountEnemiesInRange(700) >= MinQ)
                    {
                        Q.Cast();
                    }
                }
                if (useW && W.IsReady() && W.IsInRange(target))
                {
                    var Wpred = W.GetPrediction(target);
                    if (Wpred.HitChance >= HitChance.Medium)
                    {
                        W.Cast(Wpred.CastPosition);
                    }
                }
                if (useE && E.IsReady() && E.IsInRange(target) && target.HasBuff("twitchdeadlyvenom"))
                {
                    if (HarassMenu["haras" + target.ChampionName].Cast<CheckBox>().CurrentValue && Stack(target) >= MinE)
                    {
                        E.Cast();
                    }
                }
            }
        }

        private static void Damage(EventArgs args)
        {
            foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsValid && e.IsHPBarRendered && e.TotalShieldHealth() > 10))
            {
                var damage = EDamage(enemy);
                if (Misc["Damage"].Cast<CheckBox>().CurrentValue && E.IsReady() && enemy.HasBuff("twitchdeadlyvenom"))
                {
                    var dmgPer = (enemy.TotalShieldHealth() - damage > 0 ? enemy.TotalShieldHealth() - damage : 0) / (enemy.MaxHealth + enemy.AllShield + enemy.AttackShield + enemy.MagicShield);
                    var currentHPPer = enemy.TotalShieldHealth() / enemy.TotalShieldMaxHealth();
                    var initPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + dmgPer * Width), (int)enemy.HPBarPosition.Y + YOff);
                    var endPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + currentHPPer * Width) + 1, (int)enemy.HPBarPosition.Y + YOff);
                    EloBuddy.SDK.Rendering.Line.DrawLine(System.Drawing.Color.Orange, Thick, initPoint, endPoint);
                }
            }
        }
        private static float EDamage(Obj_AI_Base target)
        {
            var stacks = Stack(target);
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, BDamage[E.Level] + stacks * (0.25f * _Player.FlatPhysicalDamageMod + 0.2f * _Player.FlatMagicDamageMod + SDamage[E.Level]));
        }

        private static int Stack(Obj_AI_Base obj)
        {
            var Ec = 0;
            for (var t = 1; t < 7; t++)
            {
                if (ObjectManager.Get<Obj_GeneralParticleEmitter>().Any(s => s.Position.Distance(obj.ServerPosition) <= 175 && s.Name == "twitch_poison_counter_0" + t + ".troy"))
                {
                    Ec = t;
                }
            }
            return Ec;
        }

        public static void KillSteal()
        {
            var KsE = KillStealMenu["KsE"].Cast<CheckBox>().CurrentValue;
            var KsW = KillStealMenu["KsW"].Cast<CheckBox>().CurrentValue;
            var Eranh = Misc["ERanh"].Cast<CheckBox>().CurrentValue;
            var Eranhs = Misc["ERanhs"].Cast<Slider>().CurrentValue;
            foreach (var target in EntityManager.Heroes.Enemies.Where(hero => hero.IsValidTarget(E.Range) && !hero.HasBuff("BlitzcrankManaBarrierCD") && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsZombie))
            {
                if (KsE && E.IsReady() && target.IsValidTarget(E.Range) && target.HasBuff("twitchdeadlyvenom"))
                {
                    if (target.Health + target.AttackShield < EDamage(target))
                    {
                        E.Cast();
                    }
                }
                if (KsW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if (target.Health + target.AttackShield < Player.Instance.GetSpellDamage(target, SpellSlot.W))
                    {
                        var Wpred = W.GetPrediction(target);
                        if (Wpred.HitChancePercent >= 70)
                        {
                            W.Cast(Wpred.CastPosition);
                        }
                    }
                }
                if (Eranh && E.IsReady() && target.HasBuff("twitchdeadlyvenom"))
                {
                    if (Stack(target) >= Eranhs && 900 <= target.Distance(Player.Instance))
                    {
                        E.Cast();
                    }
                }
                if (E.IsReady() && Player.Instance.HealthPercent <= 15)
                {
                    E.Cast();
                }
                if (Ignite != null && KillStealMenu["ign"].Cast<CheckBox>().CurrentValue && Ignite.IsReady() && target.IsValidTarget(Ignite.Range))
                {
                    if (target.Health + target.AttackShield < _Player.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite))
                    {
                        Ignite.Cast(target);
                    }
                }
            }
        }
    }
}
