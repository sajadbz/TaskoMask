﻿using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskoMask.Application.Workspace.Boards.Queries.Models;
using TaskoMask.Application.Core.Dtos.Workspace.Boards;
using TaskoMask.Application.Core.Exceptions;
using TaskoMask.Application.Core.Queries;
using TaskoMask.Application.Core.Resources;
using TaskoMask.Application.Queries.Models.Boards;
using TaskoMask.Application.Core.Notifications;
using TaskoMask.Domain.Core.Resources;
using TaskoMask.Domain.Workspace.Data;
using TaskoMask.Application.Core.Helpers;
using TaskoMask.Domain.Team.Data;

namespace TaskoMask.Application.Workspace.Boards.Queries.Handlers
{
    public class BoardQueryHandlers : BaseQueryHandler,
        IRequestHandler<GetBoardByIdQuery, BoardBasicInfoDto>,
        IRequestHandler<GetBoardReportQuery, BoardReportDto>,
        IRequestHandler<GetBoardsByProjectIdQuery, IEnumerable<BoardBasicInfoDto>>,
        IRequestHandler<GetBoardsByOrganizationIdQuery, IEnumerable<BoardBasicInfoDto>>,
        IRequestHandler<SearchBoardsQuery, PublicPaginatedListReturnType<BoardOutputDto>>


    {
        #region Fields

        private readonly IBoardRepository _boardRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IProjectRepository _projectRepository;

        #endregion

        #region Ctors


        public BoardQueryHandlers(IBoardRepository boardRepository, IDomainNotificationHandler notifications, IMapper mapper, IProjectRepository projectRepository, ICardRepository cardRepository) : base(mapper, notifications)
        {
            _boardRepository = boardRepository;
            _projectRepository = projectRepository;
            _cardRepository = cardRepository;
        }


        #endregion

        #region Handlers


        /// <summary>
        /// 
        /// </summary>
        public async Task<BoardBasicInfoDto> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardRepository.GetByIdAsync(request.Id);
            if (board == null)
                throw new ApplicationException(ApplicationMessages.Data_Not_exist, DomainMetadata.Board);

            return _mapper.Map<BoardBasicInfoDto>(board);
        }



        /// <summary>
        /// 
        /// </summary>
        public async Task<IEnumerable<BoardBasicInfoDto>> Handle(GetBoardsByProjectIdQuery request, CancellationToken cancellationToken)
        {
            var boards = await _boardRepository.GetListByProjectIdAsync(request.ProjectId);
            return _mapper.Map<IEnumerable<BoardBasicInfoDto>>(boards);
        }

        


        /// <summary>
        /// 
        /// </summary>
        public async Task<IEnumerable<BoardBasicInfoDto>> Handle(GetBoardsByOrganizationIdQuery request, CancellationToken cancellationToken)
        {
            var boards = await _boardRepository.GetListByOrganizationIdAsync(request.OrganizationId);
            return _mapper.Map<IEnumerable<BoardBasicInfoDto>>(boards);
        }


        /// <summary>
        /// 
        /// </summary>
        public async Task<BoardReportDto> Handle(GetBoardReportQuery request, CancellationToken cancellationToken)
        {
            //TODO Implement GetBoardReportQuery
            return new BoardReportDto();
        }



        /// <summary>
        /// 
        /// </summary>
        public async Task<PublicPaginatedListReturnType<BoardOutputDto>> Handle(SearchBoardsQuery request, CancellationToken cancellationToken)
        {
            var boards = _boardRepository.Search(request.Page, request.RecordsPerPage, request.Term, out var pageNumber, out var totalCount);
            var boardsDto = _mapper.Map<IEnumerable<BoardOutputDto>>(boards);

            foreach (var item in boardsDto)
            {
                var project = await _projectRepository.GetByIdAsync(item.ProjectId);
                item.ProjectName = project?.Name;
                item.CardsCount = await _cardRepository.CountByBoardIdAsync(item.Id);
            }

            return new PublicPaginatedListReturnType<BoardOutputDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                Items = boardsDto
            };
        }



        #endregion

    }
}
