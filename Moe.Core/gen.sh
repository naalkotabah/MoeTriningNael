#!/bin/bash

PROJECT_NAME="${PWD##*/}"
PROJECT_PATH="$PWD"

#Vars
#   Templates
ENTITY_TEMPLATE=$(<"$PROJECT_PATH/Gen/Templates/entity")
CONTROLLER_TEMPLATE=$(<"$PROJECT_PATH/Gen/Templates/controller")
SERVICE_TEMPLATE=$(<"$PROJECT_PATH/Gen/Templates/service")
DTO_TEMPLATE=$(<"$PROJECT_PATH/Gen/Templates/dto")
#   Templates Save Path
ENTITY_TEMPLATE_SAVE_PATH="$PROJECT_PATH/Models/Entities"
CONTROLLER_TEMPLATE_SAVE_PATH="$PROJECT_PATH/Controllers"
SERVICE_TEMPLATE_SAVE_PATH="$PROJECT_PATH/Services"
DTO_TEMPLATE_SAVE_PATH="$PROJECT_PATH/Models/DTOs"
#   Snippets
MAPPER_SNIPPET=$(<"$PROJECT_PATH/Gen/Snippets/auto_mapper")
DB_CONTEXT_SNIPPET=$(<"$PROJECT_PATH/Gen/Snippets/db_context")
SERVICE_SNIPPET=$(<"$PROJECT_PATH/Gen/Snippets/service")
#   Snippets Save Path
MAPPER_SNIPPET_SAVE_PATH="$PROJECT_PATH/Helpers/AutoMapperProfile.cs"
DB_CONTEXT_SNIPPET_SAVE_PATH="$PROJECT_PATH/Data/MasterDbContext.cs"
SERVICE_SNIPPET_SAVE_PATH="$PROJECT_PATH/Extensions/AppServicesExtensions.cs"

#Methods
#   Usage: generate [EntityName]
generate(){
  ENTITY_NAME="$1"
  ENTITY_NAME_PLUR="$(pluralize $ENTITY_NAME)"
  ENTITY_NAME_CC="$(camelCase $ENTITY_NAME)"
  ENTITY_NAME_CC_PLUR="$(pluralize $ENTITY_NAME_CC)"
  
  genFile "$ENTITY_TEMPLATE" "$ENTITY_TEMPLATE_SAVE_PATH" "${ENTITY_NAME}"
  genFile "$CONTROLLER_TEMPLATE" "$CONTROLLER_TEMPLATE_SAVE_PATH" "${ENTITY_NAME_PLUR}Controller"
  genFile "$SERVICE_TEMPLATE" "$SERVICE_TEMPLATE_SAVE_PATH" "${ENTITY_NAME_PLUR}Service"
  genFile "$DTO_TEMPLATE" "$DTO_TEMPLATE_SAVE_PATH" "${ENTITY_NAME}DTO"
  
  genSnippet "$MAPPER_SNIPPET" "$MAPPER_SNIPPET_SAVE_PATH" "INSERTION_POINT"
  genSnippet "$DB_CONTEXT_SNIPPET" "$DB_CONTEXT_SNIPPET_SAVE_PATH" "INSERTION_POINT"
  genSnippet "$SERVICE_SNIPPET" "$SERVICE_SNIPPET_SAVE_PATH" "INSERTION_POINT"
}
#   Usage: genFile [Template] [SavePath] [FileName]
genFile(){
  local template=$1
  local savePath=$2
  local fileName=$3
  
  local content="$(mapContent "$template")"
  save "$fileName" "$content" "$savePath"
}

#   Usage: genSnippet [Snippet] [Path] [Key]
genSnippet(){
  local snippet=$1
  local path=$2
  local key=$3
  
  local content="$(mapContent "$snippet")"
  
  saveSnippet "$key" "$content" "$path"

  
  local val_escaped=$(printf '%s\n' "$val" | sed 's:[\/&]:\\&:g;$!s/$/\\/')

	sed -i "/{{$key}}/a\\$(echo "$val_escaped")" "$path"
}

#Helpers
#   Usage: camelCase [Name]
camelCase() {
    local input="$1"
    local camelCase=$(sed 's/_\([a-z]\)/\u\1/g; s/-\([a-z]\)/\u\1/g' <<< "$input")
    echo "${camelCase,}"
}
#   Usage: pluralize [Name]
pluralize() {
  local word="$1"

  if [[ "$word" =~ [^aeiou]y$ ]]; then
    echo "${word%y}ies"
  elif [[ "$word" =~ (s|x|z|ch|sh)$ ]]; then
    echo "${word}es"
  else
    echo "${word}s"
  fi
}
#   Usage: mapContent [Content]
mapContent() {
  local content="$1"
  
  content="$(replaceKey "PROJECT_NAME" "$PROJECT_NAME" "$content")"
  content="$(replaceKey "ENTITY_NAME" "$ENTITY_NAME" "$content")"
  content="$(replaceKey "ENTITY_NAME_PLUR" "$ENTITY_NAME_PLUR" "$content")"
  content="$(replaceKey "ENTITY_NAME_CC" "$ENTITY_NAME_CC" "$content")"
  content="$(replaceKey "ENTITY_NAME_CC_PLUR" "$ENTITY_NAME_CC_PLUR" "$content")"
  echo "$content"
}
#   Usage: replaceKey [Key] [Value] [Content]
replaceKey(){
  local key="$1"
  local val="$2"
  local content="$3"

  content=${content//\{\{"$key"\}\}/"$val"}
  echo "$content"
}
#   Usage: save [Name] [Content] [Path]
save(){
  local name="$1"
  local content="$2"
  local path="$3"
  
  echo "$content" > "${path}/${name}.cs"
}
#   Usage: saveSnippet [Key] [Val] [Path]
saveSnippet() {
	local key=$1
	local val=$2
	local path=$3

	local val_escaped=$(printf '%s\n' "$val" | sed 's:[\/&]:\\&:g;$!s/$/\\/')

	sed -i "/{{$key}}/a\\$(echo "$val_escaped")" "$path"
}

#Entry
generate $1