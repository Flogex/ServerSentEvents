﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServerSentEvents
{
    public class Server
    {
        private readonly Dictionary<ClientId, HttpContext> _clients = new Dictionary<ClientId, HttpContext>();

        public async Task<ClientId> AddClient(HttpContext context)
        {
            var id = ClientId.NewClientId();
            _clients.Add(id, context);

            var response = context.Response;
            response.StatusCode = 200;
            response.ContentType = "text/event-stream";
            response.Headers.Add("Cache-Control", "no-cache");
            await response.StartAsync();

            return id;
        }

        public Task SendEvent(ClientId id, Event @event)
        {
            if (!_clients.TryGetValue(id, out var client))
                throw new ArgumentException($"Unknown client with id {id}.");

            return EventSerializer.WriteEvent(client.Response.Body, @event);
        }

        public Task SendComment(ClientId id, string comment)
        {
            if (!_clients.TryGetValue(id, out var client))
                throw new ArgumentException($"Unknown client with id {id}.");

            return EventSerializer.WriteComment(client.Response.Body, comment);
        }
    }
}
