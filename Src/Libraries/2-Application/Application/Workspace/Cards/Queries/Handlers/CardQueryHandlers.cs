﻿using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskoMask.Application.Workspace.Cards.Queries.Models;
using TaskoMask.Application.Core.Dtos.Workspace.Cards;
using TaskoMask.Application.Core.Exceptions;
using TaskoMask.Application.Core.Queries;
using TaskoMask.Application.Core.Resources;
using TaskoMask.Application.Core.Notifications;
using TaskoMask.Domain.Core.Resources;
using TaskoMask.Domain.Workspace.Data;
using TaskoMask.Application.Core.Helpers;

namespace TaskoMask.Application.Workspace.Cards.Queries.Handlers
{
    public class CardQueryHandlers : BaseQueryHandler,
        IRequestHandler<GetCardByIdQuery, CardBasicInfoDto>,
        IRequestHandler<GetCardReportQuery, CardReportDto>,
         IRequestHandler<GetCardsByBoardIdQuery, IEnumerable<CardBasicInfoDto>>,
        IRequestHandler<SearchCardsQuery, PublicPaginatedListReturnType<CardOutputDto>>

    {
        #region Fields

        private readonly ICardRepository _cardRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly ITaskRepository _taskRepository;


        #endregion

        #region Ctors

        public CardQueryHandlers(ICardRepository cardRepository, IDomainNotificationHandler notifications, IMapper mapper, ITaskRepository taskRepository, IBoardRepository boardRepository) : base(mapper, notifications)
        {
            _cardRepository = cardRepository;
            _taskRepository = taskRepository;
            _boardRepository = boardRepository;
        }

        #endregion

        #region Handlers



        /// <summary>
        /// 
        /// </summary>
        public async Task<CardBasicInfoDto> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
        {
            var card = await _cardRepository.GetByIdAsync(request.Id);
            if (card == null)
                throw new ApplicationException(ApplicationMessages.Data_Not_exist, DomainMetadata.Card);

            return _mapper.Map<CardBasicInfoDto>(card);
        }



        /// <summary>
        /// 
        /// </summary>
        public async Task<IEnumerable<CardBasicInfoDto>> Handle(GetCardsByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var cards = await _cardRepository.GetListByBoardIdAsync(request.BoardId);
            return _mapper.Map<IEnumerable<CardBasicInfoDto>>(cards);
        }



        /// <summary>
        /// 
        /// </summary>
        public async Task<CardReportDto> Handle(GetCardReportQuery request, CancellationToken cancellationToken)
        {
            //TODO Implement GetCardReportQuery
            return new CardReportDto();
        }




        /// <summary>
        /// 
        /// </summary>
        public async Task<PublicPaginatedListReturnType<CardOutputDto>> Handle(SearchCardsQuery request, CancellationToken cancellationToken)
        {
            var cards = _cardRepository.Search(request.Page, request.RecordsPerPage, request.Term, out var pageNumber, out var totalCount);
            var cardsDto = _mapper.Map<IEnumerable<CardOutputDto>>(cards);

            foreach (var item in cardsDto)
            {
                var board = await _boardRepository.GetByIdAsync(item.BoardId);
                item.BoardName = board?.Name;
                item.TasksCount = await _taskRepository.CountByCardIdAsync(item.Id);
            }

            return new PublicPaginatedListReturnType<CardOutputDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                Items = cardsDto
            };
        }



        #endregion
    }
}
