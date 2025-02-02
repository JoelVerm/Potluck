echo -e '\x1b[1;5;97;43m Open the site at https://localhost \x1b[0m'

pnpm i -r --config.confirmModulesPurge=false

(
    (cd Backend/ && dotnet watch --quiet --project Main) &
    (cd Frontend/ && pnpm start --host --logLevel error) &
    (cd Frontend\ -\ Wall\ Screen/ && pnpm start --host --logLevel error) &
    (caddy run || ./caddy.exe run) ;
kill 0)
