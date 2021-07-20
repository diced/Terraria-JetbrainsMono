using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace JetbrainsMonoFont
{
	public class JetbrainsMonoFont : Mod
	{
		private TaskCompletionSource<bool> unloadTCS;
		private bool hasBackups;
		private DynamicSpriteFont[] combatTextBackup;
		private DynamicSpriteFont deathTextBackup;
		private DynamicSpriteFont itemStackBackup;
		private DynamicSpriteFont mouseTextBackup;
		
		public override void Load()
		{
			if(Main.dedServ) {
				return;
			}

			LoadFonts();

			Main.OnPreDraw += PreDraw;
		}
		public override void Unload()
		{
			if(Main.dedServ) {
				return;
			}

			unloadTCS = new TaskCompletionSource<bool>();

			unloadTCS.Task.Wait(); //Graphics have to be unloaded in PreDraw on the main thread, otherwise they may get unloaded as they're used.

			Main.OnPreDraw -= PreDraw;
		}

		private void PreDraw(GameTime obj)
		{
			if(unloadTCS!=null) {
				UnloadGraphics();

				unloadTCS.SetResult(true);

				unloadTCS = null;
			}
		}
		private void UnloadGraphics()
		{
			UnloadFonts();
		}
		
		private void LoadFonts()
		{
			combatTextBackup = Main.fontCombatText;
			deathTextBackup = Main.fontDeathText;
			itemStackBackup = Main.fontItemStack;
			mouseTextBackup = Main.fontMouseText;
			hasBackups = true;

			Main.fontItemStack = GetFont("Fonts/Item_Stack");
			Main.fontMouseText = GetFont("Fonts/Mouse_Text");
			Main.fontDeathText = GetFont("Fonts/Death_Text");
			Main.fontCombatText = new[] {
				GetFont("Fonts/Combat_Text"),
				GetFont("Fonts/Combat_Crit")
			};
		}
		private void UnloadFonts()
		{
			if(!hasBackups) {
				return;
			}

			Main.fontCombatText = combatTextBackup;
			Main.fontDeathText = deathTextBackup;
			Main.fontItemStack = itemStackBackup;
			Main.fontMouseText = mouseTextBackup;

			combatTextBackup = null;
			deathTextBackup = null;
			itemStackBackup = null;
			mouseTextBackup = null;

			hasBackups = false;
		}
	}
}