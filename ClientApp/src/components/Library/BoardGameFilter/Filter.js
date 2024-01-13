import { useCallback, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import FilterInput from './FilterInput';
import FilterWeightRangeInput from './FilterWeightRangeInput';
import { AppliedFilterButton } from './AppliedFilterButton';
import FilterData from './FilterData';
import FilterCriterion from './FilterCriterion';

export default Filter;
export function Filter({ filter, setFilter, collection, onFilter, getBoardGame }) {
    const [searchParams, setSearchParams] = useSearchParams();
    const updateSearchParams = useCallback(filter => {
        var newSearchParams = structuredClone(searchParams);
        for (var c of filter.getCriteria()) {
            if (c.isDefault()) {
                newSearchParams.delete(c.name);
            } else {
                newSearchParams.set(c.name, c.getValue());
            }
        }
        if (searchParams != newSearchParams) {
            setSearchParams(newSearchParams);
        }
    }, [searchParams, setSearchParams]);

    // Filter games whenever filter is updated
    useEffect(() => {
        const filteredCollection = filterBoardGames(collection, filter, getBoardGame);
        onFilter(filteredCollection);
    }, [collection, filter, getBoardGame]);

    // Synchronize search param whenever filter changes
    useEffect(() => {
        updateSearchParams(filter);
    }, [updateSearchParams, filter]);

    const updateFilter = nameValuePairs => {
        var newFilter = filter.clone();
        nameValuePairs.forEach(([name, value]) => newFilter.setValue(name, value));
        setFilter(newFilter);
    }

    return (
        <div className="bgdb-filter">
            <div className="mb-3 filter-input input-group">
                <label className="bgdb-label input-group-text">Players</label>
                <FilterInput type="number"
                    param={filter.getValue("players")}
                    handleChange={e => updateFilter([["players", e.target.value]])} />
            </div>
            <div className="mb-3 filter-input">
                <label className="bgdb-label">Complexity</label>
                <FilterWeightRangeInput filter={filter}
                    handleChange={e => updateFilter([
                        ["minWeight", e.minValue],
                        ["maxWeight", e.maxValue]
                    ])} />
            </div>
            {!filter.isDefault() &&
                <div className="bgdb-applied-filters">
                    <h5>Filters</h5>
                    <div className="container d-flex">
                        {filter.getActiveCriteria().map((c, i) => (
                            <AppliedFilterButton
                                key={i}
                                criterion={c}
                                onClose={() => updateFilter([[c.name, undefined]])} />
                        ))}
                    </div>
                </div>
            }
        </div>
    );
}

function filterBoardGames(collection, filter, getBoardGame = x => x) {
    return collection.filter(item => filter.test(getBoardGame(item)));
}

export function createEmptyFilter() {
    var filter = new FilterData();
    filter.addCriterion(new FilterCriterion({
        name: "players",
        mapValue: v => (v > 0) ? v : undefined,
        test: (g, v) => (v >= g.minPlayers) && (v <= g.maxPlayers),
        describe: v => `${v} player${v == 1 ? "" : "s"}`
    }));

    filter.addCriterion(new FilterCriterion({
        name: "minWeight",
        mapValue: v => (v > 1.0) ? v : undefined,
        test: (g, v) => g.averageWeight >= v,
        describe: v => `Complexity ≥ ${Number(v).toFixed(1)}`
    }));

    filter.addCriterion(new FilterCriterion({
        name: "maxWeight",
        mapValue: v => (v < 5.0) ? v : undefined,
        test: (g, v) => g.averageWeight <= v,
        describe: v => `Complexity ≤ ${Number(v).toFixed(1)}`
    }));
    return filter;
}

export function createFilter(searchParams) {
    var filter = createEmptyFilter();
    filter.getCriteria().forEach(c => c.setValue(searchParams.get(c.name)));
    return filter;
}
