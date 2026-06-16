using Microsoft.Extensions.AI;

namespace Parley.Providers;

public interface IChatClientProvider
{
    IChatClient Provide();
}