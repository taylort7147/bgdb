import { useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Card, CardBody, Collapse } from 'reactstrap';

function BoardGameFilterInput({ displayName, param, type }) {
    const [searchParams, setSearchParams] = useSearchParams();

    const updateSearchParams = (paramName, paramValue) => {
        var newSearchParams = structuredClone(searchParams);
        if (paramValue === undefined || paramValue === null || paramValue.length === 0) {
            newSearchParams.delete(paramName);
        } else {
            newSearchParams.set(paramName, paramValue);
        }
        setSearchParams(newSearchParams);
    };

    return (
        <div className="mb-3 input-group filter-input">
            <label className="input-group-text">{displayName}</label>
            <input className="form-control"
                type={type}
                value={searchParams.get(param) ?? ""}
                onChange={e => updateSearchParams(param, e.target.value)}></input>
        </div>
    );
}

export function BoardGameFilter() {
    return (
        <div className="filter">
            <BoardGameFilterInput displayName="Players" param="players" type="number" />
            <BoardGameFilterInput displayName="Min Complexity" param="minWeight" type="number" />
            <BoardGameFilterInput displayName="Max Complexity" param="maxWeight" type="number" />
            <BoardGameFilterInput displayName="Min Age" param="minAge" type="number" />
        </div>
    );
}

export function BoardGameFilterCollapse() {
    const [collapsed, setCollapsed] = useState(true);

    const toggle = () => setCollapsed(!collapsed);

    return (
        <div>
            <p className="d-flex g-0">
                <button className="btn btn-primary bgdb-btn-filter" onClick={toggle}>
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-filter bgdb-filter-icon" viewBox="0 0 16 16">
                        <path d="M6 10.5a.5.5 0 0 1 .5-.5h3a.5.5 0 0 1 0 1h-3a.5.5 0 0 1-.5-.5m-2-3a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 0 1h-7a.5.5 0 0 1-.5-.5m-2-3a.5.5 0 0 1 .5-.5h11a.5.5 0 0 1 0 1h-11a.5.5 0 0 1-.5-.5" />
                    </svg>
                    Filter
                </button>
            </p>
            <Collapse isOpen={!collapsed}>
                <Card className="filter-card mb-3">
                    <CardBody>
                        <BoardGameFilter />
                    </CardBody>
                </Card>
            </Collapse>
        </div>
    );
}
