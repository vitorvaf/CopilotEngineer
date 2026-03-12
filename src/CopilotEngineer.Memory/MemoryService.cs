using CopilotEngineer.Core;

namespace CopilotEngineer.Memory;

public sealed class MemoryService(
    ProjectContextLoader projectContextLoader,
    ConventionsLoader conventionsLoader,
    BugMemoryRepository bugMemoryRepository) : IContextProvider
{
    public async Task<EngineerContext> BuildAsync(UserRequest request, Intent intent, CancellationToken cancellationToken = default)
    {
        var projectContext = await projectContextLoader.LoadAsync(cancellationToken);
        var conventions = await conventionsLoader.LoadAsync(cancellationToken);
        var relatedBugs = await bugMemoryRepository.SearchRelevantAsync(request.Input, cancellationToken: cancellationToken);

        return new EngineerContext(
            projectContext.ProjectName,
            projectContext.ArchitectureSummary,
            projectContext.Workflows,
            projectContext.Tools,
            projectContext.Stack,
            conventions.Naming,
            conventions.Patterns,
            relatedBugs);
    }
}
