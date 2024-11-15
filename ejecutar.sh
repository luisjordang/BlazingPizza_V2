#!/bin/bash

docker stop blazingpizza-container 2>/dev/null
docker rm blazingpizza-container 2>/dev/null

docker build -t blazingpizza-image .

docker run -d --network host --name blazingpizza-container blazingpizza-image

