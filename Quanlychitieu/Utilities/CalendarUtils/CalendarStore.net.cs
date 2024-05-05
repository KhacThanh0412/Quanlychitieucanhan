using Microsoft.Maui.Graphics;

namespace Plugin.Maui.CalendarStore;

partial class CalendarStoreImplementation : ICalendarStore
{
	public Task<string> CreateAllDayEventImplement(string calendarId, string title, string description,
		string location, DateTimeOffset startDate, DateTimeOffset endDate)
	{
		throw new NotImplementedException();
	}

	public Task<string> CreateEventImplement(string calendarId, string title, string description,
		string location, DateTimeOffset startDateTime, DateTimeOffset endDateTime,
		bool isAllDay = false)
	{
		throw new NotImplementedException();
	}

	public Task<string> CreateEventWithReminderImplement(string calendarId, string title, string description, string location, DateTimeOffset startDateTime, DateTimeOffset endDateTime, int reminderMinutes, bool isAllDay = false)
	{
		throw new NotImplementedException();
	}
	public Task<string> CreateEventImplement(CalendarEvent calendarEvent)
	{
		throw new NotImplementedException();
	}

	public Task<Calendar> GetCalendarImplement(string calendarId)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<Calendar>> GetCalendarsImplement()
	{
		throw new NotImplementedException();
	}

	public Task<string> CreateCalendarImplement(string name, Color? color = null)
	{
		throw new NotImplementedException();
	}

	//public Task DeleteCalendar(string calendarId)
	//{
	//	throw new NotImplementedException();
	//}

	//public Task DeleteCalendar(Calendar calendarToDelete)
	//{
	//	throw new NotImplementedException();
	//}

	public Task<CalendarEvent> GetEventImplement(string eventId)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<CalendarEvent>> GetEventsImplement(string? calendarId = null,
		DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
	{
		throw new NotImplementedException();
	}

	public Task DeleteEventImplement(string eventId)
	{
		throw new NotImplementedException();
	}

	public Task DeleteEventImplement(CalendarEvent eventToDelete)
	{
		throw new NotImplementedException();
	}

	public Task UpdateCalendarImplement(string calendarId, string newName, Color? newColor = null)
	{
		throw new NotImplementedException();
	}

	public Task UpdateEventImplement(string eventId, string title, string description,
		string location, DateTimeOffset startDateTime, DateTimeOffset endDateTime, bool isAllDay)
	{
		throw new NotImplementedException();
	}

	public Task UpdateEventImplement(CalendarEvent eventToUpdate)
	{
		throw new NotImplementedException();
	}

	public Task UpdateEventWithReminderImplement(string eventId, string title, string description, string location, DateTimeOffset startDateTime, DateTimeOffset endDateTime, bool isAllDay, int reminderMinutes) => throw new NotImplementedException();
}