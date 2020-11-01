using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Slot.BackOffice.Data.Queries.Filters;
using Slot.BackOffice.Filters;
using Slot.Core.Data;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slot.BackOffice.Controllers
{
    [ApiVersion("1.0")]
    [BackOfficeAuthorize]
    [ValidateQueryOperator]
    public class FiltersController : BaseController
    {
        private readonly CachedSettings cachedSettings;

        public FiltersController(CachedSettings cachedSettings)
        {
            this.cachedSettings = cachedSettings;
        }

        [HttpGet]
        public IActionResult Operators([FromQuery]OperatorQuery query)
        {
            var operators = GetOperators(query);

            return GetResult(operators.Select(@operator => new SelectListItem
            {
                Value = Convert.ToString(@operator.Id),
                Text = @operator.Name
            }));
        }

        [HttpGet]
        public IActionResult Games([FromQuery]GameQuery query)
        {
            var games = GetGames(query);

            return GetResult(games.Select(game => new SelectListItem
            {
                Value = Convert.ToString(game.Id),
                Text = game.Name
            }));
        }

        [HttpGet]
        public IActionResult Currencies()
        {
            return GetResult(cachedSettings.CurrenciesByIsoCode
                                            .Select(kv => kv.Value)
                                            .Where(currency => !currency.IsDeleted && currency.IsVisible)
                                            .OrderBy(currency => currency.IsoCode)
                                            .Select(currency => new SelectListItem
                                            {
                                                Value = Convert.ToString(currency.Id),
                                                Text = currency.IsoCode
                                            }));
        }

        private IEnumerable<Operator> GetOperators(OperatorQuery query)
        {
            IEnumerable<Operator> operators = null;

            if (query.OperatorId.HasValue)
            {
                operators = cachedSettings.OperatorsById
                                .Where(kv => kv.Key == query.OperatorId.Value)
                                .Select(kv => kv.Value);
            }
            else
            {
                operators = cachedSettings.OperatorsById
                                .Select(kv => kv.Value);
            }

            return operators;
        }

        private IEnumerable<Operator> MoveW88ToTop(IEnumerable<Operator> operators)
        {
            var operatorList = operators.ToList();
            var w88 = operatorList.First(@operator => @operator.Tag == "w88");

            operatorList.Remove(w88);
            operatorList.Insert(0, w88);
            return operatorList;
        }

        private IEnumerable<Game> GetGames(GameQuery query)
        {
            IEnumerable<Game> games = null;
            if (query.OperatorId.HasValue)
            {
                games = cachedSettings.Games
                        .Select(kv => kv.Value)
                        .Where(game => !game.IsDisabled && !game.DisableOperators.Contains(Convert.ToString(query.OperatorId)));
            }
            else
            {
                games = cachedSettings.Games
                        .Select(kv => kv.Value)
                        .Where(game => !game.IsDisabled);
            }

            return games
                    .OrderBy(game => game.Name);
        }
    }
}
