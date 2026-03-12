namespace CopilotEngineer.Workflows;

internal sealed record WorkflowDefinition(
    string Name,
    IReadOnlyList<WorkflowStepDefinition> Steps);

internal sealed record WorkflowStepDefinition(
    string Name,
    string SpecialistName,
    string Instruction);
