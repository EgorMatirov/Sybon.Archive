using System.Collections.Generic;
using Sybon.Archive.Services.ProblemsService.Models;

namespace Sybon.Archive.Services.InternalProblemsService
{
    public interface IInternalProblemsService
    {
        Problem FetchProblemInfo(Problem problem);
        string ExtractStatementUrl(Problem problem);
        bool Exists(params string[] problemsIds);
    }
}