//  Raptor - a client API for Terraria
//  Copyright (C) 2013-2014 MarioE
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Raptor.Api
{
	/// <summary>
	/// Represents the different packet types.
	/// </summary>
	public enum PacketTypes : byte
	{
		/// <summary>
		/// The connection request packet.
		/// </summary>
		ConnectRequest = 1,
		/// <summary>
		/// The disconnect packet.
		/// </summary>
		Disconnect,
		/// <summary>
		/// The continue connecting packet.
		/// </summary>
		ContinueConnecting,
		/// <summary>
		/// The player info packet. Contains name, colors, difficulty setting, etc.
		/// </summary>
		PlayerInfo,
		/// <summary>
		/// The player inventory packet. Contains item slot and prefix.
		/// </summary>
		PlayerInvSlot,
		/// <summary>
		/// The second continue connecting packet.
		/// </summary>
		ContinueConnecting2,
		/// <summary>
		/// The world info packet. Contains the bosses that have been defeated, serversidecharacter status, etc.
		/// </summary>
		WorldInfo,
		/// <summary>
		/// The world section request packet.
		/// </summary>
		GetWorldSection,
		/// <summary>
		/// The client status packet.
		/// </summary>
		Status,
		/// <summary>
		/// The send world section packet.
		/// </summary>
		SendWorldSection,
		/// <summary>
		/// The world section frame packet.
		/// </summary>
		WorldSectionFrame,
		/// <summary>
		/// The spawn player packet.
		/// </summary>
		SpawnPlayer,
		/// <summary>
		/// The update player packet. Contains player movement flags, pulley status, etc.
		/// </summary>
		UpdatePlayer,
		/// <summary>
		/// The active player packet. Sets a player as active.
		/// </summary>
		SetActivePlayer,
		/// <summary>
		/// The sync players packet.
		/// </summary>
		SyncPlayers,
		/// <summary>
		/// The player hp packet. Contains hp and max hp.
		/// </summary>
		SetPlayerHp,
		/// <summary>
		/// The modify tile packet.
		/// </summary>
		ModifyTile,
		/// <summary>
		/// The set time packet.
		/// </summary>
		SetTime,
		/// <summary>
		/// The use door packet.
		/// </summary>
		UseDoor,
		/// <summary>
		/// The send tile square packet.
		/// </summary>
		SendTileSquare,
		/// <summary>
		/// The item drop update packet.
		/// </summary>
		UpdateItem,
		/// <summary>
		/// The item owner update packet.
		/// </summary>
		SetItemOwner,
		/// <summary>
		/// The npc update packet.
		/// </summary>
		UpdateNpc,
		/// <summary>
		/// The strike npc with item packet.
		/// </summary>
		StrikeNpcItem,
		/// <summary>
		/// The chat packet.
		/// </summary>
		Chat,
		/// <summary>
		/// The damage player packet.
		/// </summary>
		DamagePlayer,
		/// <summary>
		/// The update projectile packet.
		/// </summary>
		UpdateProjectile,
		/// <summary>
		/// The strike npc packet.
		/// </summary>
		StrikeNpc,
		/// <summary>
		/// The destroy projectile packet.
		/// </summary>
		DestroyProjectile,
		/// <summary>
		/// The player pvp status packet.
		/// </summary>
		SetPlayerPvp,
		/// <summary>
		/// The get chest packet.
		/// </summary>
		GetChest,
		/// <summary>
		/// The set chest item slot packet.
		/// </summary>
		SetChestItem,
		/// <summary>
		/// The set current chest packet.
		/// </summary>
		SetCurrentChest,
		/// <summary>
		/// The remove chest packet.
		/// </summary>
		RemoveChest,
		/// <summary>
		/// The player healing effect packet.
		/// </summary>
		PlayerHealEffect,
		/// <summary>
		/// The player biome zone packet.
		/// </summary>
		SetPlayerZone,
		/// <summary>
		/// The request password packet.
		/// </summary>
		RequestPassword,
		/// <summary>
		/// The send password packet.
		/// </summary>
		SendPassword,
		/// <summary>
		/// The disown item packet.
		/// </summary>
		DisownItem,
		/// <summary>
		/// The set talking npc packet.
		/// </summary>
		SetTalkNpc,
		/// <summary>
		/// The player animation packet.
		/// </summary>
		AnimatePlayer,
		/// <summary>
		/// The player mana packet. Contains mana and max mana.
		/// </summary>
		SetPlayerMana,
		/// <summary>
		/// The player mana effect packet.
		/// </summary>
		PlayerManaEffect,
		/// <summary>
		/// The kill player packet.
		/// </summary>
		KillPlayer,
		/// <summary>
		/// The player team packet.
		/// </summary>
		SetPlayerTeam,
		/// <summary>
		/// The get sign packet.
		/// </summary>
		GetSign,
		/// <summary>
		/// The sign update packet.
		/// </summary>
		UpdateSign,
		/// <summary>
		/// The modify liquit packet.
		/// </summary>
		ModifyLiquid,
		/// <summary>
		/// The first-spawn packet.
		/// </summary>
		FirstSpawn,
		/// <summary>
		/// The update player buffs packet.
		/// </summary>
		UpdatePlayerBuffs,
		/// <summary>
		/// The npc special packet.
		/// </summary>
		SpecialNpc,
		/// <summary>
		/// The unlock chest packet.
		/// </summary>
		UnlockChest,
		/// <summary>
		/// The add npc buff packet.
		/// </summary>
		AddNpcBuff,
		/// <summary>
		/// The update npc buffs packet.
		/// </summary>
		UpdateNpcBuffs,
		/// <summary>
		/// The add player buff packet.
		/// </summary>
		AddPlayerBuff,
		/// <summary>
		/// The npc name packet.
		/// </summary>
		SetNpcName,
		/// <summary>
		/// The good vs. evil percentage packet.
		/// </summary>
		SetGoodEvilPercent,
		/// <summary>
		/// The musical instrument packet.
		/// </summary>
		PlayMusic,
		/// <summary>
		/// The activate wire packet.
		/// </summary>
		ActivateWire,
		/// <summary>
		/// The npc home packet.
		/// </summary>
		SetNpcHome,
		/// <summary>
		/// The boss or invasion spawn packet.
		/// </summary>
		SpawnBossInvasion,
		/// <summary>
		/// The player dodge packet.
		/// </summary>
		PlayerDodge,
		/// <summary>
		/// The paint tile packet.
		/// </summary>
		PaintTile,
		/// <summary>
		/// The paint wall packet.
		/// </summary>
		PaintWall,
		/// <summary>
		/// The teleport packet.
		/// </summary>
		Teleport,
		/// <summary>
		/// The heal player packet.
		/// </summary>
		HealPlayer,
		/// <summary>
		/// The raptor packet (formerly the placeholder packet).
		/// </summary>
		Raptor,
		/// <summary>
		/// The uuid packet.
		/// </summary>
		Uuid
	}
}
