
function renderRange(a, b) {
    a = a ?? b;
    b = b ?? a;
    if (a != null) {
        var min = Math.min(a, b);
        var max = Math.max(a, b);
        if (min === max) {
            return <>{min}</>;
        } else {
            return <>{min}-{max}</>
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
            {/* Card body */}
            <div className="card-body">
                <h3 className="card-title">{game.name}</h3>
                <p className="card-text"><small className="text-body-secondary">Some other text</small></p>
            </div>

            {/* Game info */}
            <div className="rounded-bottom bgdb-summary d-grid">
                <div className="row g-0 justify-content-start text-start">

                    {/* Thumbnail */}
                    <div className="col-auto">
                        <div className="bgdb-card-thumbnail-container ">
                            <img className="img-fluid bgdb-card-thumbnail" src={`/api/asset/img/${game.thumbnailId}`} alt="" />
                        </div>
                    </div>

                    {/* Details */}
                    <div className="col-auto p-2 d-grid text-nowrap">

                        {/* Players */}
                        <div className="row row-cols-2 g-0">
                            <div className="col-auto">
                                <img src="/img/icon/meeple.png" className="bgdb-icon" alt="" />
                            </div>
                            <div className="col ps-2 my-auto">
                                <span>{renderRange(game.minPlayers, game.maxPlayers)}</span>
                            </div>
                        </div>

                        {/* Play time */}
                        <div className="row row-cols-2 g-0">
                            <div className="col-auto">
                                <img src="/img/icon/clock.png" className="bgdb-icon" alt="" />
                            </div>
                            <div className="col ps-2 my-auto">
                                <span>{renderRange(game.minPlayTimeMinutes, game.maxPlayTimeMinutes)}</span>
                            </div>
                        </div>

                        {/* Complexity */}
                        <div className="row row-cols-2 g-0">
                            <div className="col-auto">
                                <img src="/img/icon/think.png" className="bgdb-icon" alt="" />
                            </div>
                            <div className="col ps-2 my-auto">
                                {renderWeight(game.averageWeight, 2)}
                            </div>
                        </div>

                        {/* Location */}
                        <div className="row row-cols-2 g-0">
                            <div className="col-auto">
                                <img src="/img/icon/location.png" className="bgdb-icon" alt="" />
                            </div>
                            <div className="col ps-2 my-auto">
                                <span>A7</span>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    );
}
