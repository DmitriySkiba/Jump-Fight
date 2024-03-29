using Godot;
using System;

public partial class LevelGenerator : Node2D
{
	private PackedScene BattleSection;
	[Export]
	private int platform_count = 0;
	public int num_levels = 6;

	private float last_platform_pos_y = 0;
	private Camera2D camera;
	private player Player;

	public PackedScene JumpPlatform_scene = new PackedScene();
	public PackedScene ChestPlatform_scene = new PackedScene();
	public PackedScene BreackablePlatform_scene = new PackedScene();
	public PackedScene TriggerOnEnterBattle = new PackedScene();
	public PackedScene TriggerOnExitBattle = new PackedScene();
	public override void _Ready()
	{
		BattleSection = GD.Load<PackedScene>("res://scenes/game/LevelItems/Rooms/Room1.tscn");
		JumpPlatform_scene = GD.Load<PackedScene>("res://scenes/game/LevelItems/Platforms/JumpPlatform/JumpPlatform.tscn");
		ChestPlatform_scene = GD.Load<PackedScene>("res://scenes/game/LevelItems/Platforms/ChestPlatform.tscn");
		BreackablePlatform_scene = GD.Load<PackedScene>("res://scenes/game/LevelItems/Platforms/BreackablePlatform/BreackablePlatform.tscn");
		TriggerOnEnterBattle = GD.Load<PackedScene>("res://scenes/game/Utils/TriggerOnEnterBattle.tscn");
		TriggerOnExitBattle = GD.Load<PackedScene>("res://scenes/game/Utils/TriggerOnExitBattle.tscn");
		Player = GetParent().GetNode<player>("Player");
	
		_spawn_levels();
	}

	public override void _Process(double delta)
	{
	}

	private void _spawn_levels(){
		Node2D prev_battle_room = new Node2D();
		Node2D cur_battle_room = new Node2D();
		string prev_room_name = "";
		for (int i = 0; i < num_levels; i++){
			if (i == 0){
				GeneratePlatform(Player.Position.Y, 10);
				prev_room_name = "platform";
			}else if(prev_room_name == "platform"){
				cur_battle_room = BattleSection.Instantiate<Room1>();
				cur_battle_room.Position = new Vector2(cur_battle_room.Position.X, last_platform_pos_y - 200);
				prev_battle_room = cur_battle_room;
				prev_room_name = "battle";

				Node2D EnterTrigger = TriggerOnEnterBattle.Instantiate<TriggerOnEnterBattle>();
				EnterTrigger.Position = new Vector2(cur_battle_room.Position.X, cur_battle_room.Position.Y - 500);

				Node2D ExitTrigger = TriggerOnExitBattle.Instantiate<TriggerOnExitBattle>();
				ExitTrigger.Position = new Vector2(ExitTrigger.Position.X, prev_battle_room.GetNode<Marker2D>("NextPlatform").GlobalPosition.Y - 200);
				this.AddChild(cur_battle_room);
				this.AddChild(EnterTrigger);
				this.AddChild(ExitTrigger);
			}else{
				float init_pos_y = prev_battle_room.GetNode<Marker2D>("NextPlatform").GlobalPosition.Y;
				GeneratePlatform(init_pos_y, 30);
				prev_room_name = "platform";
			}
		}
	}

	private void _physics_process(float delta)
	{
		
	}

	private void GeneratePlatform(float initial_pos_y, int amount){
		Random rnd = new Random();
		for (int i = 0; i < amount; i++){
			int random_y = rnd.Next(120, 170);
			int random_x = rnd.Next(700, 1225);

			initial_pos_y -= random_y;
			JumpPlatform new_platform = JumpPlatform_scene.Instantiate<JumpPlatform>();
			new_platform.Position = new Vector2(random_x, initial_pos_y);
			last_platform_pos_y = initial_pos_y;
			this.AddChild(new_platform);
		}
	}

}
