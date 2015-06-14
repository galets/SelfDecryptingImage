#!/bin/bash

uglifyjs Resources/init.js > Resources/init.min.js
uglifyjs Resources/aes.js > Resources/aes.min.js
uglifyjs Resources/b64.js > Resources/b64.min.js
