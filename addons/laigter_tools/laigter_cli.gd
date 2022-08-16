tool
extends Node
class_name LaigterCli

# cli flags - `laigter --help` output is pasted at the bottom for reference
const CONFIG_KEYS_CLI = [LTConfig.ConfigKeys.GENERATE_NORMAL_MAP, 
	LTConfig.ConfigKeys.GENERATE_OCCLUSION, 
	LTConfig.ConfigKeys.GENERATE_PARALLAX,
	LTConfig.ConfigKeys.GENERATE_SPECULAR, 
	LTConfig.ConfigKeys.HIDE_LAIGTER_GUI]

const os_command = {
	"Windows": { "console": "cmd.exe", 	"flag": "/c" }, 
	"X11": { "console": "/usr/bin/bash", "flag": "-c" },
	"OSX": { "console": "/usr/bin/bash", "flag": "-c" } # ? is this correct? 
}

# get a string representing the optional flags to pass to the laigter binary from settings
static func get_cmd_flags() -> String:
	var flags = PoolStringArray()
	var defaults = LTConfig.get_config_defaults()
	for setting_idx in LTConfig.ConfigKeys:
		if LTConfig.get_config_value(LTConfig.ConfigKeys[setting_idx]) and defaults[LTConfig.ConfigKeys[setting_idx]].has("cli_flag"):
			flags.append(defaults[LTConfig.ConfigKeys[setting_idx]]["cli_flag"])
	return flags.join(" ")

# execute a process
# if blocking, returns exit code, else PID
static func execute_cmd(cmd: String, blocking = true) -> int:
	var native = os_command[OS.get_name()]
	var output = []
	print("Executing: %s %s %s" % [native.console, native.flag, cmd])
	var exit_code_or_pid = OS.execute(native.console, [native.flag, cmd], blocking, output, true)
	if (blocking and exit_code_or_pid != 0):
		print("Process exited with code: %d" % exit_code_or_pid)
	for line in output:
		print(line)
	return exit_code_or_pid

# given a Resource, get the full OS file path from the res:// path
static func get_absolute_resource_path(res_path: String):
	var filesystem = File.new()
	var err = filesystem.open(res_path, File.READ)	
	if (err != OK):
		print("%s: Failed load resource from %s" % [err, res_path])
		filesystem.close()
		return null	
	var abs_path = filesystem.get_path_absolute()
	if (OS.get_name() == "X11"):
		abs_path = '"%s"' % abs_path
	filesystem.close()
	return abs_path
	
static func get_preset(preset_name: String):
	return "-r %s" % get_absolute_resource_path("%s/%s" % [LTConfig.PRESETS_DEFAULT_PATH, preset_name])

static func execute_laigter(input_texture: Texture, preset: String = "") -> LaigterCliResult:
	var result_obj = LaigterCliResult.new()
	if (input_texture != null):
		var dir = Directory.new()
		
		result_obj.file_hash = input_texture.resource_path.hash()
		result_obj.cache_dir = LTConfig.get_cache_path(result_obj.file_hash)
		result_obj.cache_file_path = "%s/%s" % [result_obj.cache_dir, input_texture.resource_path.get_file()]
		result_obj.input_file = input_texture.resource_path
		
		assert(dir.file_exists(input_texture.resource_path), "failed find source texture at %s" % input_texture.resource_path)
		assert(dir.dir_exists(result_obj.cache_dir) or dir.make_dir_recursive(result_obj.cache_dir) == OK, "failed to create cache dir: %s" % result_obj.cache_dir)
		assert(dir.copy(input_texture.resource_path, result_obj.cache_file_path) == OK, "failed to copy texture resource to tmp user:// dir")
		
		print("copying '%s' from\n'%s' to '%s'" % [input_texture.resource_path.get_file(), input_texture.resource_path.get_base_dir(), result_obj.cache_file_path])
		
		var command = "{laigter_binary} --no-gui {flags} {preset} -d {input_texture}".format({
		 "laigter_binary": LTConfig.get_config_value(LTConfig.ConfigKeys.LAIGTER_BINARY_PATH), 
		 "flags": get_cmd_flags(), 
		 "input_texture": get_absolute_resource_path(result_obj.cache_file_path),
		 "preset": get_preset(preset) if preset != "" else ""
		})
		result_obj.exit_code = execute_cmd(command, true)
		
		if (LTConfig.get_config_value(LTConfig.ConfigKeys.HIDE_LAIGTER_GUI) != true):
			execute_laigter_gui(input_texture, preset)
	return result_obj
	
static func execute_laigter_gui(input_texture: Texture, preset: String = ""):
	var file_hash = input_texture.resource_path.hash()
	var cache_file_path = "%s/%s" % [LTConfig.get_cache_path(file_hash), input_texture.resource_path.get_file()]
	var gui_command = "{laigter_binary} {flags} {preset} -d {input_texture}".format({
		 "laigter_binary": LTConfig.get_config_value(LTConfig.ConfigKeys.LAIGTER_BINARY_PATH), 
		 "flags": get_cmd_flags(), 
		 "input_texture": get_absolute_resource_path(cache_file_path),
		 "preset": get_preset(preset) if preset != "" else ""
		})
	return execute_cmd(gui_command, false)

#	└─▪ laigter --help
#	Usage: laigter [options]
#	Test helper
#
#	Options:
#	-h, --help									 Displays help on commandline options.
#	--help-all									 Displays help including Qt specific
#	options.
#	-v, --version								 Displays version information.
#	-s, --software-opengl						Use software opengl renderer.
#	-g, --no-gui									do not start graphical interface
#	-d, --diffuse <diffuse texture path>	diffuse texture to load
#	-n, --normal									generate normals
#	-c, --specular								generate specular
#	-o, --occlusion								generate occlusion
#	-p, --parallax								generate parallax
#	-r, --preset <preset file path>		 presset to load
