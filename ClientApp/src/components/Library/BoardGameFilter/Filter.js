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
        setSearchParams(newSearchParams);
    }, [searchParams, setSearchParams]);

    useEffect(() => {
        updateSearchParams([
            ["players", filter.getPlayers()],
            ["minWeight", filter.getMinWeight()],
            ["maxWeight", filter.getMaxWeight()]
        ]);
    }, [filter, updateSearchParams]);

    const setPlayers = value => {
        console.log(`setPlayers(${value})`);
        var newFilter = filter.clone();
        newFilter.setPlayers(value);
        setFilter(newFilter);
    }

    const setWeightRange = (min, max) => {
        var newFilter = filter.clone();
        newFilter.setMinWeight(min);
        newFilter.setMaxWeight(max);
        setFilter(newFilter);
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
