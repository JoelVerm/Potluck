echo -e '\x1b[1;5;97;43m Open the site at https://localhost \x1b[0m'

(
    (cd Backend/ && dotnet watch --quiet) &
    (cd Frontend/ && pnpm start --host --logLevel error) &
    (cd Frontend\ -\ Wall\ Screen/ && pnpm start --host --logLevel error) &
    (./caddy.exe run > /dev/null 2>&1) ;
kill 0)
