# Après avoir modifié le Dockerfile
git add frontend/depensio.Web/Dockerfile
git commit -m "fix: Replace API_SETTINGS_URL placeholder in appsettings.json during build"
git push
```

**3. Relancez le workflow de build :**

Allez dans **Actions** → **Build and Push Docker Images** → **Run workflow**

**4. Une fois la nouvelle image buildée, redéployez :**

Relancez le workflow **Deploy images to dev**

**5. Vérifiez dans les logs du build que le sed s'exécute :**

Dans les logs GitHub Actions, vous devriez voir :
```
Step X/Y : RUN sed -i "s|${API_SETTINGS_URL}|https://apidemo.depensio.com|g" /app/wwwroot/appsettings.json