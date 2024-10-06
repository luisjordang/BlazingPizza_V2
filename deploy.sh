#!/bin/bash

# Detener y eliminar contenedores existentes si existen
docker stop blazingpizza-container 2>/dev/null
docker rm blazingpizza-container 2>/dev/null

# Construir la imagen Docker para la aplicación Blazor
docker build -t blazingpizza-image .

docker run -d --network host --name blazingpizza-container blazingpizza-image

# chmod +x monitoreo.sh
# ./monitoreo.sh

###################################################################################
# echo "Aplicación BlazingPizza en ejecución:"
# echo "http://localhost:8080"

