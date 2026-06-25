# Offer Service

## Descrizione

L'**Offer Service** è un microservizio responsabile della gestione delle offerte effettuate sugli immobili.

Consente agli utenti di creare offerte sugli immobili disponibili e ai proprietari degli immobili di accettare o rifiutare le offerte ricevute.

## Architettura

Il servizio fa parte di un'architettura a microservizi:

* Espone API REST
* Tutte le operazioni sono protette tramite autenticazione JWT
* Comunica con il **Property Service** per verificare disponibilità e proprietà degli immobili
* Comunica in modo asincrono tramite Apache Kafka per notificare gli eventi relativi alle offerte

## Avvio del servizio

Per avviare il servizio in locale:

```bash id="v7d4pq"
# esempio
dotnet run
```

Oppure con Docker:

```bash id="s9k2mx"
docker-compose up
```

Il servizio sarà disponibile su:

```id="n5u8ra"
http://localhost:7803
```

## API

Documentazione Swagger disponibile qui:

```id="q4e7zt"
http://localhost:7803/swagger/index.html
```

## Autenticazione e autorizzazione

Il servizio utilizza **JWT (JSON Web Token)** per proteggere tutte le operazioni.

### Accesso autenticato

Tutte le operazioni richiedono un token JWT valido.

Il client deve includere il token nell'header HTTP:

```http id="h3w9cy"
Authorization: Bearer <token>
```

### Regole di autorizzazione

#### Creazione di un'offerta

Un utente autenticato può creare un'offerta soltanto se:

* l'immobile esiste
* l'immobile non è stato venduto
* non è il proprietario dell'immobile

Le nuove offerte vengono create automaticamente con stato **Pending**.

#### Modifica di un'offerta

Un'offerta può essere modificata solo se:

* l'utente autenticato è l'autore dell'offerta

#### Eliminazione di un'offerta

Un'offerta può essere eliminata solo se:

* l'utente autenticato è l'autore dell'offerta

#### Accettazione di un'offerta

Un'offerta può essere accettata solo se:

* l'utente autenticato è il proprietario dell'immobile associato
* l'immobile è ancora disponibile
* l'offerta è in stato **Pending**

Quando un'offerta viene accettata:

* il suo stato diventa **Accepted**
* tutte le altre offerte pendenti sullo stesso immobile vengono automaticamente invalidate

#### Rifiuto di un'offerta

Un'offerta può essere rifiutata solo se:

* l'utente autenticato è il proprietario dell'immobile associato

Quando un'offerta viene rifiutata:

* il suo stato diventa **Rejected**

## Stati di un'offerta

Un'offerta può assumere i seguenti stati:

| Stato    | Descrizione                        |
| -------- | ---------------------------------- |
| Pending  | Offerta in attesa di una decisione |
| Accepted | Offerta accettata dal proprietario |
| Rejected | Offerta rifiutata dal proprietario |
| Expired  | Offerta scaduta automaticamente    |

## Gestione automatica delle scadenze

Le offerte hanno una durata limitata.

* Alla creazione viene impostata una scadenza a 30 giorni.
* Le offerte scadute vengono automaticamente marcate come **Expired**.
* Le offerte scadute non possono essere accettate.

## Endpoints principali

| Metodo | Endpoint               | Autenticazione | Descrizione                |
| ------ | ---------------------- | -------------- | -------------------------- |
| GET    | /api/Offer             | ✅ Sì           | Recupera tutte le offerte  |
| GET    | /api/Offer/{id}        | ✅ Sì           | Recupera un'offerta per ID |
| POST   | /api/Offer             | ✅ Sì           | Crea una nuova offerta     |
| PUT    | /api/Offer/{id}        | ✅ Sì           | Aggiorna un'offerta        |
| DELETE | /api/Offer/{id}        | ✅ Sì           | Elimina un'offerta         |
| PATCH  | /api/Offer/{id}/accept | ✅ Sì           | Accetta un'offerta         |
| PATCH  | /api/Offer/{id}/reject | ✅ Sì           | Rifiuta un'offerta         |

## Integrazioni

### Property Service

L'Offer Service utilizza il Property Service per:

* verificare l'esistenza dell'immobile
* verificare la disponibilità dell'immobile
* recuperare il proprietario dell'immobile
* impedire offerte su immobili venduti
* impedire offerte sui propri immobili

### Kafka

#### Eventi Kafka pubblicati

| Topic        | Evento        | Descrizione                                |
| ------------ | ------------- | ------------------------------------------ |
| offer-events | OfferCreated  | Notifica la creazione di una nuova offerta |
| offer-events | OfferAccepted | Notifica l'accettazione di un'offerta      |
| offer-events | OfferRejected | Notifica il rifiuto di un'offerta          |

## Controlli automatici

* L'autore dell'offerta viene associato automaticamente all'offerta al momento della creazione.
* Le offerte vengono create con stato iniziale **Pending**.
* Le offerte hanno una scadenza automatica di 30 giorni.
* Le offerte scadute vengono automaticamente marcate come **Expired**.
* Quando un'offerta viene accettata, tutte le altre offerte pendenti sullo stesso immobile vengono automaticamente invalidate.
* Tutti i controlli di autorizzazione si basano sull'utente autenticato contenuto nel JWT.
