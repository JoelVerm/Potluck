# Security en access control

Het beveiligen van een webapp bestaat uit veel onderdelen, maar een belangrijke die vaak over het hoofd wordt gezien of waar nog problemen in zitten is access control. Dit houdt in dat een aanvaller data kan zien die niet zichtbaar moet zijn zonder specifieke toestemming. Helaas komt dit probleem zo vaak voor dat het op de OWASP-lijst plaats A01 heeft (1). Daarom is het belangrijk om te weten wat Broken Access Control is, en hoe je het kunt voorkomen.

## Wat is Broken Access Control?

Broken Access Control omvat elke beveiligingsfout waarbij data te bereiken is zonder de juiste toestemmingen. Dit kan door onveilige logingegevens, maar ook door simpelweg de url van een API-aanroep aan te kunnen passen. Ook het vergeten te controleren op authenticatie bij bepaalde endpoints valt hieronder.

Een aantal bekende voorbeelden zijn een hack op Facebook waarbij een functie waarvoor je ingelogd moest zijn zichtbaar was voor andere gebruikers (2) en een aanval op Microsoft Exchange waar arbitrary code execution plaatsvond doordat de proxy en de backend inconsistente beveiliging hadden (3).

Ook komt het voor dat hackers toegang krijgen tot bestanden buiten de webroot door `../` in een bestandsnaam te zetten, of een verhulde versie hiervan. Bij het lezen van de bestandsnaam wordt een map te ver terug gelezen waar gevoelige informatie kan staan. (4)

Een ander voorbeeld is een fout in toegangsbeheer waarbij gebruikers hun eigen permissies konden verhogen door bijvoorbeeld de loginparameters in een HTTP-verzoek te wijzigen naar een adminaccount. Als de server deze wijzigingen niet controleert, krijgen gebruikers onjuist toegang tot beheerdersfuncties.

## Hoe voorkom je Broken Access Control?

Om Broken Access Control te voorkomen kun je deze stappen ondernemen:

- Minimaliseer toegangsrechten: Geef gebruikers alleen toegang tot wat ze strikt nodig hebben. Beperk de rechten van accounts zoveel mogelijk.
- Beveilig API's en endpoints: Zorg ervoor dat alle verzoeken naar API's en endpoints gecontroleerd worden op zowel authenticatie als autorisatie. Elk verzoek moet geverifieerd worden.
- Voorkom directory traversal: Gebruik veilige methoden voor bestandsbeheer. Controleer ingevoerde paden om te voorkomen dat gebruikers buiten de webroot toegang krijgen.
- Controleer instellingen: Controleer regelmatig de configuratie van servers om kwetsbaarheden door verkeerde instellingen te voorkomen.
- Audit toegangspogingen. Houd logs bij van toegangspogingen, vooral bij verdachte aanvragen of bij een hoge rate. Dit helpt bij het snel detecteren en oplossen van problemen.

## Bronnen

1. https://owasp.org/Top10/A01_2021-Broken_Access_Control/
2. https://arstechnica.com/information-technology/2018/09/50-million-facebook-accounts-breached-by-an-access-token-harvesting-attack/
3. https://devco.re/blog/2021/08/06/a-new-attack-surface-on-MS-exchange-part-1-ProxyLogon/
4. https://cwe.mitre.org/data/definitions/22.html