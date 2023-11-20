amespace Services.IServices
{
    public interface IEventGridService
    {
        public Task<string> SendEventAsync(MessageingModel eventData);
    }
}