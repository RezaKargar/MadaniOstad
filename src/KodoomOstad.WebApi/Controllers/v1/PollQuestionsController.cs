using AutoMapper;
using KodoomOstad.DataAccessLayer.Contracts;
using KodoomOstad.Entities.Models;
using KodoomOstad.WebApi.Models.Answers;
using KodoomOstad.WebApi.Models.PollQuestions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KodoomOstad.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    public class PollQuestionsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<PollQuestion> _pollQuestionRepository;

        public PollQuestionsController(IMapper mapper, IRepository<PollQuestion> pollQuestionRepository)
        {
            _mapper = mapper;
            _pollQuestionRepository = pollQuestionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var pollQuestions = await _pollQuestionRepository.TableNoTracking.ToListAsync(cancellationToken);
            var dtos = _mapper.Map<List<PollQuestionOutputDto>>(pollQuestions);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var pollQuestion = await _pollQuestionRepository.GetByIdAsync(cancellationToken, id);

            if (pollQuestion == null)
                return NotFound();

            var dto = _mapper.Map<PollQuestionOutputDto>(pollQuestion);
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PollQuestionInputDto dto, CancellationToken cancellationToken)
        {
            NormalizeScores(ref dto);

            var pollQuestion = _mapper.Map<PollQuestion>(dto);

            await _pollQuestionRepository.AddAsync(pollQuestion, cancellationToken);

            var createdPollQuestion = _mapper.Map<PollQuestionOutputDto>(pollQuestion);

            return Created($"api/v1/PollQuestions/{createdPollQuestion.Id}", createdPollQuestion);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, PollQuestionInputDto dto, CancellationToken cancellationToken)
        {
            var pollQuestion = await _pollQuestionRepository.GetByIdAsync(cancellationToken, id);

            if (pollQuestion == null)
                return NotFound();

            NormalizeScores(ref dto);

            _mapper.Map(dto, pollQuestion);

            await _pollQuestionRepository.UpdateAsync(pollQuestion, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var pollQuestion = await _pollQuestionRepository.GetByIdAsync(cancellationToken, id);

            if (pollQuestion == null)
                return NotFound();

            await _pollQuestionRepository.DeleteAsync(pollQuestion, cancellationToken);

            return NoContent();
        }

        [HttpGet("{id}/Answers")]
        public async Task<IActionResult> Answers(int id, CancellationToken cancellationToken)
        {
            var pollQuestion = await _pollQuestionRepository.GetByIdAsync(cancellationToken, id);

            if (pollQuestion == null)
                return NotFound();

            await _pollQuestionRepository.LoadCollectionAsync(pollQuestion, pq => pq.Answers, cancellationToken);

            var answers = _mapper.Map<List<AnswersOutputDto>>(pollQuestion.Answers);

            return Ok(answers);
        }

        [HttpGet("{pollQuestionId}/Answers/{answerId}")]
        public async Task<IActionResult> Answers(int pollQuestionId, int answerId, CancellationToken cancellationToken)
        {
            var pollQuestion = await _pollQuestionRepository.GetByIdAsync(cancellationToken, pollQuestionId);

            if (pollQuestion == null)
                return NotFound("PollQuestion not found.");

            await _pollQuestionRepository.LoadCollectionAsync(pollQuestion, pq => pq.Answers, cancellationToken);

            var answer = pollQuestion.Answers.SingleOrDefault(a => a.Id == answerId);

            if (answer == null)
                return NotFound("Answer of  PollQuestion not found.");

            var dto = _mapper.Map<AnswersOutputDto>(answer);
            return Ok(dto);
        }

        private static void NormalizeScores(ref PollQuestionInputDto dto)
        {
            if (dto.MaxScore >= dto.MinScore) return;
            var temp = dto.MaxScore;
            dto.MaxScore = dto.MinScore;
            dto.MinScore = temp;
        }
    }
}
