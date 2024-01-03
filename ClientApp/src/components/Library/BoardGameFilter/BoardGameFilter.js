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
            <p>
                <button className="btn btn-primary" onClick={toggle}>Filter</button>
            </p>
            <Collapse isOpen={!collapsed}>
                <Card className="filter-card">
                    <CardBody>
                        <BoardGameFilter />
                    </CardBody>
                </Card>
            </Collapse>
        </div>
    );
}
