#!/bin/bash
echo "Generando token JWT para pruebas..."
echo

dotnet script scripts/generate-test-token.cs

echo
echo "Token generado exitosamente!"