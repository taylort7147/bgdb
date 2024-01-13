export class FilterData {
    constructor() {
        this.criteria = new Map();
    }

    clone() {
        var filter = new FilterData();
        filter.criteria = new Map();
        this.criteria.forEach((criterion, name) => filter.criteria.set(name, criterion.clone()));
        return filter;
    }

    equals(other) {
        const mapsAreEqual = (m1, m2) => m1.size === m2.size && Array.from(m1.keys()).every(key => m1.get(key) === m2.get(key));
        return mapsAreEqual(this.criteria, other.criteria);
    }

    addCriterion(criterion) {
        this.criteria.set(criterion.name, criterion);
    }

    getCriterion(name) {
        return this.criteria.get(name);
    }

    getValue(name) {
        return this.getCriterion(name)?.getValue();
    }

    setValue(name, value) {
        this.getCriterion(name)?.setValue(value);
    }

    isDefault() {
        return Array.from(this.criteria.values()).every(c => c.isDefault());
    }

    test(boardGame) {
        return Array.from(this.criteria.values()).every(c => c.test(boardGame));
    }

    getActiveCriteria() {
        return this.getCriteria().filter(c => !c.isDefault());
    }

    getCriteria() {
        return Array.from(this.criteria.values());
    }
}

export default FilterData;
