using System;

namespace CQRS_EventSourcing.Events.Interfaces;

public interface IEvent
{
    Guid EventGuid { get; }
}