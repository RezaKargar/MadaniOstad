﻿using AutoMapper;
using KodoomOstad.DataAccessLayer.Contracts;
using KodoomOstad.Entities.Models;
using KodoomOstad.WebApi.Models.Faculties;
using KodoomOstad.WebApi.Models.Professors;
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
    public class FacultiesController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Faculty> _facultyRepository;

        public FacultiesController(IMapper mapper, IRepository<Faculty> facultyRepository)
        {
            _mapper = mapper;
            _facultyRepository = facultyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var faculties = await _facultyRepository.Entities.ToListAsync(cancellationToken);
            var dtos = _mapper.Map<List<FacultyOutputDto>>(faculties);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var faculty = await _facultyRepository.GetByIdAsync(cancellationToken, id);
            if (faculty == null)
                return NotFound();

            var dto = _mapper.Map<FacultyOutputDto>(faculty);
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(FacultyInputDto dto, CancellationToken cancellationToken)
        {
            var isSlugDuplicated = await _facultyRepository.TableNoTracking.AnyAsync(f => f.Slug == dto.Slug, cancellationToken);

            if (isSlugDuplicated)
                return BadRequest("Faculty with same Slug already exists.");

            var faculty = _mapper.Map<Faculty>(dto);
            await _facultyRepository.AddAsync(faculty, cancellationToken);

            var createdFaculty = _mapper.Map<FacultyOutputDto>(faculty);

            return Created("api/v1/Faculties/" + createdFaculty.Id, createdFaculty);
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, FacultyInputDto dto, CancellationToken cancellationToken)
        {
            var faculty = await _facultyRepository.GetByIdAsync(cancellationToken, id);

            if (faculty == null)
                return NotFound();

            if (faculty.Name == dto.Name || faculty.Slug != dto.Slug)
            {
                var isSlugDuplicated = await _facultyRepository
                        .TableNoTracking
                        .AnyAsync(f => f.Name == dto.Name || f.Slug == dto.Slug, cancellationToken);

                if (isSlugDuplicated)
                    return BadRequest("Faculty with same 'Name' or 'Slug' already exists.");
            }

            _mapper.Map(dto, faculty);
            await _facultyRepository.UpdateAsync(faculty, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var faculty = await _facultyRepository.GetByIdAsync(cancellationToken, id);

            if (faculty == null)
                return NotFound();

            await _facultyRepository.DeleteAsync(faculty, cancellationToken);
            return NoContent();
        }

        [HttpGet("{id}/Professors")]
        public async Task<IActionResult> Professors(int id, CancellationToken cancellationToken)
        {
            var faculty = await _facultyRepository.GetByIdAsync(cancellationToken, id);

            if (faculty == null)
                return NotFound();

            await _facultyRepository.LoadCollectionAsync(faculty, f => f.Professors, cancellationToken);

            var professors = _mapper.Map<List<ProfessorOutputDto>>(faculty.Professors);
            return Ok(professors);
        }

        [HttpGet("{facultyId}/Professors/{professorId}")]
        public async Task<IActionResult> Professors(int facultyId, int professorId, CancellationToken cancellationToken)
        {
            var faculty = await _facultyRepository.GetByIdAsync(cancellationToken, facultyId);

            if (faculty == null)
                return NotFound("Faculty not found.");

            await _facultyRepository.LoadCollectionAsync(faculty, f => f.Professors, cancellationToken);

            var professor = faculty.Professors.SingleOrDefault(p => p.Id == professorId);

            if (professor == null)
                return NotFound("Professor of faculty not found.");

            var dto = _mapper.Map<ProfessorOutputDto>(professor);
            return Ok(dto);
        }
    }
}