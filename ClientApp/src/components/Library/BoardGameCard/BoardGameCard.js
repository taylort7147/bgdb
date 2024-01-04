
function renderRange(a, b) {
    a = a ?? b;
    b = b ?? a;
    if (a != null) {
        var min = Math.min(a, b);
        var max = Math.max(a, b);
        if (min === max) {
            return <span>{min}</span>;
        } else {
            return <span>{min}-{max}</span>
        }
    }
};

function renderWeight(weight, decimalPlaces) {
    const roundingPoint = 1.0 / Math.pow(10, decimalPlaces)
    const roundedWeight = Math.trunc(Math.round((weight + roundingPoint / 2) / roundingPoint)) * roundingPoint;
    const intWeight = Math.trunc(weight);
    const className = `bgdb-weight-${intWeight}`;
    return <span className={className}>{roundedWeight.toFixed(decimalPlaces)}</span>
}

export function BoardGameCard({ game }) {
    return (
        <div className="card mb-3">
            <div className="row g-0">
                {/* Card body */}
                <div className="col">
                    <div className="card-body">
                        <h3 className="card-title">{game.name}</h3>
                        <p className="card-text">Some other text</p>
                        <p className="card-text"><small className="text-body-secondary">Some other text</small></p>
                    </div>
                </div>

                {/* Game info */}
                <div className="col-auto">
                    <div className="row g-0">
                        <div className="col-auto bgdb-summary d-grid">
                            <div className="row g-0">
                                <div className="col-auto">
                                    <span>2-7</span>
                                </div>
                                <div className="col-auto ms-1">
                                    <img src="/img/icon/meeple.png" className="bgdb-small-icon" />
                                </div>
                            </div>
                            <div className="row g-0">
                                <div className="col-auto">
                                    <span>{renderRange(game.minPlayTimeMinutes, game.maxPlayTimeMinutes)}</span>
                                </div>
                                <div className="col-auto ms-1">
                                    <img src="/img/icon/clock.png" className="bgdb-small-icon" />
                                </div>
                            </div>
                            <div className="row g-0">
                                <div className="col-auto">
                                    {renderWeight(game.averageWeight, 2)}
                                </div>
                                <div className="col-auto ms-1">
                                    <img src="/img/icon/think.png" className="bgdb-small-icon" />
                                </div>
                            </div>
                            <div className="row g-0">
                                <div className="col-auto">
                                    <span>A7</span>
                                </div>
                                <div className="col-auto ms-1">
                                    <img src="/img/icon/location.png" className="bgdb-small-icon" />
                                </div>
                            </div>
                        </div>

                        {/* Thumbnail */}
                        <div className="col-auto">
                            <div className="bgdb-card-thumbnail-container rounded-end">
                                <img className="img-fluid rounded-end bgdb-card-thumbnail" src={`/api/asset/img/${game.thumbnailId}`} alt="" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
