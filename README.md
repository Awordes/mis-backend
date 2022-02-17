# Mercury Integration Service
## Application deployment
   1. Make directories:
      ```
      mkdir mis
      mkdir mis/logs
      mkdir mis/files
      mkdir mis/backend
      mkdir mis/backend/publish
      mkdir mis/frontend
      mkdir postgres
      cp Dockerfile mis/backend/
      cp docker-compose.yml mis/
      cp nginx.conf mis/
      
      ```
   1. Publish backend to `mis/backend/publish`
   1. Publish frontend to `mis/frontend`