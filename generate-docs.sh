#!/bin/sh
set -e

docfx ./docs/docfx.json

SOURCE_DIR=$PWD
TEMP_REPO_DIR=$PWD/../docs-temp

echo "Удаление временной папки $TEMP_REPO_DIR"
rm -rf ${TEMP_REPO_DIR}
mkdir ${TEMP_REPO_DIR}

echo "Клонирование ветки gh-pages"
git clone https://github.com/SolarLabRU/IvorySharp.git --branch gh-pages ${TEMP_REPO_DIR}

echo "Очистка ветки"
cd ${TEMP_REPO_DIR}
git rm -r *

echo "Копирование документации"
cp -r ${SOURCE_DIR}/docs/_site/* .

git add . -A
git commit -m "Обновлена документация библиотеки"
git push origin gh-pages