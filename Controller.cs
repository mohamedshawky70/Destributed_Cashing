//Inject                      shape that your data return in it
private readonly ICashService<IEnumerable<QuestionResponse>> _cashService;
private const string cashPrefix = "allQuestion";

public async Task<IActionResult> GetAllAsync([FromRoute] int PollId, CancellationToken cancellationToken)
{
	var poll = await _unitOfWork.polls.GetByIdAsync(PollId);
	if (poll is null)
		return NotFound(PollErrors.NotFound);

	//مفتاح هعمل الكاش بيه وهحذفه وهجيبه بيه _
	//يونيك فاليو علشان ميحفظش كل الاسأله بكل البولز بتاعتها بنفس المفتاح علشان لما تجيب الاسأله بتاعة بول واحد يجيب بتاعة بول واحد بس
	var cachKey = $"{cashPrefix}-{PollId}";
	var cachedQuestion = await _cashService.GetAsync(cachKey, cancellationToken);
	IEnumerable<QuestionResponse> response = [];
	if (cachedQuestion is null)
	{
		var questions = await _unitOfWork.questions.FindAllInclude(x => x.PollId == PollId, cancellationToken, new[] { "answers" });
		response = questions.Adapt<IEnumerable<QuestionResponse>>();
		await _cashService.SetAsync(cachKey, response, cancellationToken);
	}
	else
	{
		response = cachedQuestion;
	}
	return Ok(response);
}


// In Create ,Update and Delete endpoint to get last vesion from caching
await _cashService.RemoveAsync($"{cashPrefix}-{question.PollId}", cancellationToken);
