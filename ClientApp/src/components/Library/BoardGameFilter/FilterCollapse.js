import { useState } from 'react';
import { Card, CardBody, Collapse } from 'reactstrap';
import Filter from './Filter';
import FilterData from './FilterData';

export default FilterCollapse;
export function FilterCollapse({ filter, setFilter, collection, onFilter, getBoardGame }) {
    const [collapsed, setCollapsed] = useState(filter.isDefault());
    const toggle = () => setCollapsed(!collapsed);
    const clearFilters = () => {
        setCollapsed(true);
        setFilter(new FilterData());
    }

    return (
        <div className="bgdb-filter-collapse">
            <p className="d-flex g-0">
                <button className="btn btn-primary bgdb-btn-filter me-1" onClick={toggle}>
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-filter bgdb-icon" viewBox="0 0 16 16">
                        <path d="M6 10.5a.5.5 0 0 1 .5-.5h3a.5.5 0 0 1 0 1h-3a.5.5 0 0 1-.5-.5m-2-3a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 0 1h-7a.5.5 0 0 1-.5-.5m-2-3a.5.5 0 0 1 .5-.5h11a.5.5 0 0 1 0 1h-11a.5.5 0 0 1-.5-.5" />
                    </svg>
                    Filter
                </button>
                {!filter.isDefault() && <button className="btn btn-primary" onClick={clearFilters}>Clear</button>}
            </p>
            <Collapse isOpen={!collapsed}>
                <Card className="filter-card mb-3">
                    <CardBody>
                        <Filter 
                            filter={filter} 
                            setFilter={setFilter} 
                            collection={collection}
                            onFilter={onFilter}
                            getBoardGame={getBoardGame}/>
                    </CardBody>
                </Card>
            </Collapse>
        </div>
    );
}
