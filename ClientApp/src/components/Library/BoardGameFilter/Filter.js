import { useCallback, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import FilterInput from './FilterInput';
import FilterWeightRangeInput from './FilterWeightRangeInput';

export default Filter;
export function Filter({ filter, setFilter }) {
    const [searchParams, setSearchParams] = useSearchParams();

    const updateSearchParams = useCallback(params => {
        var newSearchParams = structuredClone(searchParams);
        params.forEach(nameValuePair => {
            const [paramName, paramValue] = nameValuePair;
            if (paramValue === undefined || paramValue === null || paramValue.length === 0) {
                newSearchParams.delete(paramName);
            } else {
                newSearchParams.set(paramName, paramValue);
            }
        });
        if (searchParams != newSearchParams) {
            setSearchParams(newSearchParams);
        }
    }, [searchParams, setSearchParams]);

    useEffect(() => {
        updateSearchParams([
            ["players", filter.getPlayers()],
            ["minWeight", filter.getMinWeight()],
            ["maxWeight", filter.getMaxWeight()]
        ]);
    }, [filter, updateSearchParams]);

    const updateFilter = (newFilter) => {
        if (!filter.equals(newFilter)) {
            setFilter(newFilter);
        }
    }

    const setPlayers = value => {
        var newFilter = filter.clone();
        newFilter.setPlayers(value);
        updateFilter(newFilter);
    }

    const setWeightRange = (min, max) => {
        var newFilter = filter.clone();
        newFilter.setMinWeight(min);
        newFilter.setMaxWeight(max);
        updateFilter(newFilter);
    };

    return (
        <div className="filter">
            <div className="mb-3 filter-input input-group">
                <label className="bgdb-label input-group-text">Players</label>
                <FilterInput type="number"
                    param={filter.getPlayers()}
                    handleChange={e => setPlayers(e.target.value)} />
            </div>
            <div className="mb-3 filter-input">
                <label className="bgdb-label">Complexity</label>
                <FilterWeightRangeInput filter={filter} handleChange={e => setWeightRange(e.minValue, e.maxValue)} />
            </div>
        </div>
    );
}
