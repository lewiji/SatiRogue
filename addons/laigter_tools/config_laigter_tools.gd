tool
extends Resource
class_name LTConfig

const NAME = "laigter_config"
const TEMP_USER_DIR = "user://%s" % NAME
const TEMP_CONFIGFILE_PATH = "%s/laigter.tmp" % TEMP_USER_DIR
const TEMP_CACHE_DIR = "%s/cache" % TEMP_USER_DIR
const LAIGTER_BIN_DEFAULT_PATH = {
	"X11": "/usr/bin/laigter", 
	"Windows": "%APPDATA%\\..\\Local\\Programs\\Laigter\\laigter.exe", 
	"OSX": "/Applications/laigter.app"
}
const PRESETS_DEFAULT_PATH = "res://addons/laigter_tools/laigter_presets"
# config keys for configurable settings
enum ConfigKeys { 
	GENERATE_NORMAL_MAP = 0, 
	GENERATE_SPECULAR = 1, 
	GENERATE_OCCLUSION = 2, 
	GENERATE_PARALLAX = 3, 
	LAIGTER_BINARY_PATH = 4,
	HIDE_LAIGTER_GUI = 5,
}
# config values
static func get_config_defaults() -> Dictionary:
	return {ConfigKeys.GENERATE_NORMAL_MAP: {"type": TYPE_BOOL, "default": true, "cli_flag": "-n"}, 
		ConfigKeys.GENERATE_SPECULAR: {"type": TYPE_BOOL, "default": true, "cli_flag": "-c"}, 
		ConfigKeys.GENERATE_OCCLUSION: {"type": TYPE_BOOL, "default": true, "cli_flag": "-o"}, 
		ConfigKeys.GENERATE_PARALLAX: {"type": TYPE_BOOL, "default": true, "cli_flag": "-p"}, 
		ConfigKeys.HIDE_LAIGTER_GUI: {"type": TYPE_BOOL, "default": true}, 
		ConfigKeys.LAIGTER_BINARY_PATH: {"type": TYPE_STRING, "hint": PROPERTY_HINT_GLOBAL_FILE, 
		 "default": LAIGTER_BIN_DEFAULT_PATH[OS.get_name()]}}

static func get_cache_path(dir):
	return "%s/%s" % [TEMP_CACHE_DIR, dir]

# get full project settings key
static func get_qualified_setting_name(setting: int) -> String:
	return "%s/%s" % [NAME, ConfigKeys.keys()[setting]]

# returns configuration value from specified ConfigKeys enum
static func get_config_value(config_key: int):
	return ProjectSettings.get_setting(get_qualified_setting_name(config_key))
	
static func set_config_value(config_key: int, value):
	ProjectSettings.set_setting(get_qualified_setting_name(config_key), value)

# initialise new/load existing temp cfg, return the initialised ConfigFile
static func _load_tmp_config_file():
	var config = ConfigFile.new()
	var err = config.load(TEMP_CONFIGFILE_PATH)
	return config

# get a saved value from the temp ConfigFile
static func get_temp_config_value(key: String):
	var cfg: ConfigFile = _load_tmp_config_file()
	if (cfg != null and cfg.has_section_key(NAME, key)):
	 return cfg.get_value(NAME, key)
	
# assign a key/value pair to the temp ConfigFile, and save to disk
static func set_temp_config_value(key: String, value):
	var cfg: ConfigFile = _load_tmp_config_file()
	if (cfg != null):
	 cfg.set_value(NAME, key, value)
	 cfg.save(TEMP_CONFIGFILE_PATH)
	 
# erase tmp config
static func clear_temp_config_file():
	var config: ConfigFile = _load_tmp_config_file()
	if (config != null):
	 config.clear()
	 config.save(TEMP_CONFIGFILE_PATH)
