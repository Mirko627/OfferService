# Offer Service

## Descrizione

L'**Offer Service** è un microservizio responsabile della gestione delle offerte effettuate sugli immobili.

Fornisce operazioni CRUD (Create, Read, Update, Delete) e consente ai proprietari degli immobili di accettare o rifiutare le offerte ricevute.

## Architettura

Il servizio fa parte di un'architettura a microservizi:

* Espone API REST
* Tutte le operazioni sono protette tramite autenticazione JWT
* Comunica in modo asincrono con **Property Service** tramite Apache Kafka

## Avvio del servizio

Per avviare il servizio in locale:

```bash
# esempio
dotnet run
```

Oppure con Docker:

```bash
docker-compose up
```

Il servizio sarà disponibile su:

```
http://localhost:7803
```

## API

Documentazione Swagger disponibile qui:

```
http://localhost:7803/swagger/index.html
```

## Autenticazione e autorizzazione

Il servizio utilizza **JWT (JSON Web Token)** per proteggere tutte le operazioni.

### Accesso autenticato

Tutte le operazioni richiedono un token JWT valido:

* Creazione di un'offerta
* Visualizzazione delle offerte
* Modifica di un'offerta
* Eliminazione di un'offerta
* Accettazione di un'offerta
* Rifiuto di un'offerta

Il client deve includere il token nell'header HTTP:

```http
Authorization: Bearer <token>
```

### Regole di autorizzazione

* Un utente autenticato può:

  * creare offerte sugli immobili
  * visualizzare le offerte consentite dal sistema
  * modificare **solo le proprie offerte**
  * eliminare **solo le proprie offerte**

* Il proprietario dell'immobile può:

  * accettare le offerte ricevute sul proprio immobile
  * rifiutare le offerte ricevute sul proprio immobile

* Nessun utente può modificare o eliminare offerte create da altri utenti.

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

### Eventi Kafka pubblicati

| Topic        | Evento        | Descrizione                                                     |
| ------------ | ------------- | --------------------------------------------------------------- |
| offer-events | OfferAccepted | Notifica l'accettazione di un'offerta per aggiornare l'immobile |
| offer-events | OfferRejected | Notifica il rifiuto di un'offerta                               |

* Gli eventi vengono pubblicati quando il proprietario accetta o rifiuta un'offerta.
* Il **Property Service** utilizza tali eventi per aggiornare lo stato dell'immobile associato.
* I controlli di autorizzazione si basano sull'utente contenuto nel JWT.
