
import MultiRangeSlider from "multi-range-slider-react";

export default FilterWeightRangeInput;
function FilterWeightRangeInput({ filter, handleChange }) {
    return <MultiRangeSlider
        min={1.0}
        max={5.0}
        step={0.1}
        stepOnly
        minValue={filter.getMinWeight() || 1.0}
        maxValue={filter.getMaxWeight() || 5.0}
        onChange={handleChange}
        ruler="false"
        labels={[1, 2, 3, 4, 5]}
        className="bgdb-slider"
    />
}
