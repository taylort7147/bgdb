import { useCallback, useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import FilterInput from './FilterInput';
import FilterWeightRangeInput from './FilterWeightRangeInput';
import { AppliedFilterButton } from './AppliedFilterButton';
import FilterData from './FilterData';
import FilterCriterion from './FilterCriterion';

export default Filter;
export function Filter({ filter, setFilter, collection, onFilter, getBoardGame }) {
    const [searchParams, setSearchParams] = useSearchParams();
    const [mechanics, setMechanics] = useState([]);
    const [categories, setCategories] = useState([]);
    const [families, setFamilies] = useState([]);

    const updateSearchParams = useCallback(filter => {
        var newSearchParams = new URLSearchParams();
        for (var c of filter.getCriteria()) {
            if (c.getValue() !== undefined) {
                newSearchParams.set(c.name, c.getValue());
            }
        }
        if (searchParams !== newSearchParams) {
            setSearchParams(newSearchParams);
        }
    }, [searchParams, setSearchParams]);

    // Filter games whenever filter is updated
    useEffect(() => {
        const filteredCollection = filterBoardGames(collection, filter, getBoardGame);
        onFilter(filteredCollection);
    }, [collection, filter, onFilter, getBoardGame]);

    // Synchronize search param whenever filter changes
    useEffect(() => {
        updateSearchParams(filter);
    }, [updateSearchParams, filter]);

    // Collect all availalble game properties
    useEffect(() => {
        var tempMechanics = new Set();
        var tempCategories = new Set();
        var tempFamilies = new Set();
        for (var item of filterBoardGames(collection, filter, getBoardGame)) {
            var boardGame = getBoardGame(item);
            boardGame.mechanics.forEach(m => tempMechanics.add(m));
            boardGame.categories.forEach(c => tempCategories.add(c));
            boardGame.families.forEach(f => tempFamilies.add(f));
        }
        setMechanics([""].concat(Array.from(tempMechanics).sort()));
        setCategories([""].concat(Array.from(tempCategories).sort()));
        setFamilies([""].concat(Array.from(tempFamilies).sort()));
    }, [collection, filter, getBoardGame, setMechanics, setCategories, setFamilies]);

    const updateFilterCriteria = (criteria) => {
        var newFilter = filter.clone();
        criteria.forEach(([name, criterion]) => {
            if (criterion === undefined) {
                newFilter.removeCriterion(name);
            } else {
                newFilter.addCriterion(criterion);
            }
        });
        setFilter(newFilter);
    };

    return (
        <div className="bgdb-filter">
            <div className="mb-3 filter-input input-group">
                <label className="bgdb-label input-group-text">Players</label>
                <FilterInput type="number"
                    param={filter.getValue("players")}
                    handleChange={e => updateFilterCriteria([["players", createPlayerCriterion(e.target.value)]])}
                />
            </div>
            <div className="mb-3 filter-input">
                <label className="bgdb-label">Complexity</label>
                <FilterWeightRangeInput filter={filter}
                    handleChange={e => {
                        updateFilterCriteria([
                            ["minWeight", createMinWeightCriterion(e.minValue)],
                            ["maxWeight", createMaxWeightCriterion(e.maxValue)]
                        ])
                    }}
                />
            </div>
            <div className="mb-3 filter-input">
                <label className="bgdb-label">Mechanics</label>
                <select className="form-select"
                    onChange={e => updateFilterCriteria([["mechanic", createMechanicCriterion(e.target.value)]])}
                    value={filter.getValue("mechanic") || ""}
                >
                    {mechanics.map((m, i) => <option key={i} value={m}>{m}</option>)}
                </select>
            </div>
            <div className="mb-3 filter-input">
                <label className="bgdb-label">Categories</label>
                <select className="form-select"
                    onChange={e => updateFilterCriteria([["category", createCategoryCriterion(e.target.value)]])}
                    value={filter.getValue("category") || ""}
                >
                    {categories.map((c, i) => <option key={i} value={c}>{c}</option>)}
                </select>
            </div>
            <div className="mb-3 filter-input">
                <label className="bgdb-label">Families</label>
                <select className="form-select"
                    onChange={e => updateFilterCriteria([["family", createFamilyCriterion(e.target.value)]])}
                    value={filter.getValue("family") || ""}
                >
                    {families.map((f, i) => <option key={i} value={f}>{f}</option>)}
                </select>
            </div>
            {!filter.isDefault() &&
                <div className="bgdb-applied-filters">
                    <h5>Filters</h5>
                    <div className="container">
                        {filter.getCriteria().map((c, i) => (
                            <AppliedFilterButton
                                key={i}
                                criterion={c} // TODO: Make this a text field
                                onClose={() => updateFilterCriteria([[c.name]])} />
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

function createPlayerCriterion(players) {
    if (players > 0) {
        return new FilterCriterion({
            name: "players",
            value: players,
            test: g => (players >= g.minPlayers) && (players <= g.maxPlayers),
            description: `${players} player${players === 1 ? "" : "s"}`
        });
    }
}

function createMinWeightCriterion(minWeight) {
    if (minWeight > 1.0) {
        return new FilterCriterion({
            name: "minWeight",
            value: minWeight,
            test: g => g.averageWeight >= minWeight,
            description: `Complexity ≥ ${Number(minWeight).toFixed(1)}`
        });
    }
}

function createMaxWeightCriterion(maxWeight) {
    if (maxWeight < 5.0) {
        return new FilterCriterion({
            name: "maxWeight",
            value: maxWeight,
            test: g => g.averageWeight <= maxWeight,
            description: `Complexity ≤ ${Number(maxWeight).toFixed(1)}`
        });
    }
}

function createMechanicCriterion(mechanic) {
    if (mechanic !== "") {
        return new FilterCriterion({
            name: "mechanic",
            value: mechanic,
            test: g => g.mechanics.includes(mechanic),
            description: `Mechanic: ${mechanic}`
        });
    }
}

function createCategoryCriterion(category) {
    if (category !== "") {
        return new FilterCriterion({
            name: "category",
            value: category,
            test: g => g.categories.includes(category),
            description: `Category: ${category}`
        });
    }
}

function createFamilyCriterion(family) {
    if (family !== "") {
        return new FilterCriterion({
            name: "family",
            value: family,
            test: g => g.families.includes(family),
            description: `Family: ${family}`
        });
    }
}

export function createFilter(searchParams) {
    var filter = new FilterData();
    for (var [key, value] of searchParams) {
        if (key === "players") {
            filter.addCriterion(createPlayerCriterion(value));
        }
        else if (key === "minWeight") {
            filter.addCriterion(createMinWeightCriterion(value));
        }
        else if (key === "maxWeight") {
            filter.addCriterion(createMaxWeightCriterion(searchParams.get("maxWeight")));
        }
        else if (key === "mechanic") {
            filter.addCriterion(createMechanicCriterion(value));
        }
        else if (key === "category") {
            filter.addCriterion(createCategoryCriterion(value));
        }
        else if (key === "family") {
            filter.addCriterion(createFamilyCriterion(value));
        }
    }

    return filter;
}
