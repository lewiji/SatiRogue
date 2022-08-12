tool
extends Resource
class_name LaigterSettings

const project_settings_category = "laigter"
const laigter_default_path = {"linux": "/usr/bin/laigter"}
# Temp config file
const temp_config_path = "user://laigter.tmp"
const temp_image_path = "user://cache/"
const temp_config_section = "LaigterPlugin"

enum PROJECT_SETTING {
   GENERATE_NORMAL_MAP = 0, 
   GENERATE_SPECULAR = 1, 
   GENERATE_OCCLUSION = 2, 
   GENERATE_PARALLAX = 3, 
   LAIGTER_BINARY_PATH = 4
}

const SettingTypes: Dictionary = {
   PROJECT_SETTING.GENERATE_NORMAL_MAP: TYPE_BOOL, 
   PROJECT_SETTING.GENERATE_SPECULAR: TYPE_BOOL, 
   PROJECT_SETTING.GENERATE_OCCLUSION: TYPE_BOOL, 
   PROJECT_SETTING.GENERATE_PARALLAX: TYPE_BOOL, 
   PROJECT_SETTING.LAIGTER_BINARY_PATH: TYPE_STRING
}

const SettingPropertyHints: Dictionary = {
   PROJECT_SETTING.LAIGTER_BINARY_PATH: PROPERTY_HINT_GLOBAL_FILE
}

const SettingDefaults: Dictionary = {
   PROJECT_SETTING.GENERATE_NORMAL_MAP: true, 
   PROJECT_SETTING.GENERATE_SPECULAR: true, 
   PROJECT_SETTING.GENERATE_OCCLUSION: true, 
   PROJECT_SETTING.GENERATE_PARALLAX: true, 
   PROJECT_SETTING.LAIGTER_BINARY_PATH: "/usr/bin/laigter"
}

var config = ConfigFile.new()

# get full project settings key
static func get_qualified_setting_name(setting: int) -> String:
   return "%s/%s" % [project_settings_category, PROJECT_SETTING.keys()[setting]]

# initialise new/load existing temp cfg, return the initialised ConfigFile
func _load_tmp_config_file():
   var err = config.load(temp_config_path)
   return config

# get a saved value from the temp ConfigFile
func get_temp_config_value(key: String):
   var cfg: ConfigFile = _load_tmp_config_file()
   if (cfg != null and cfg.has_section_key(temp_config_section, key)):
      return cfg.get_value(temp_config_section, key)
   
# assign a key/value pair to the temp ConfigFile, and save to disk
func set_temp_config_value(key: String, value):
   var cfg: ConfigFile = _load_tmp_config_file()
   if (cfg != null):
      cfg.set_value(temp_config_section, key, value)
      cfg.save(temp_config_path)
      
# erase tmp config
func clear_temp_config_file():
   var config: ConfigFile = _load_tmp_config_file()
   if (config != null):
      config.clear()
      config.save(temp_config_path)
