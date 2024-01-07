import Filter from "./Filter";

export class FilterData {
    constructor(searchParams) {
        this.players = searchParams?.get("players");
        this.minWeight = searchParams?.get("minWeight");
        this.maxWeight = searchParams?.get("maxWeight");
    }

    clone() {
        var filter = new FilterData();
        filter.setPlayers(this.players);
        filter.setMinWeight(this.minWeight);
        filter.setMaxWeight(this.maxWeight);
        return filter;
    }

    equals(other){
        return this.players == other.players
            && this.minWeight == other.minWeight
            && this.maxWeight == other.maxWeight;
    }

    isDefault() {
        return this.players == undefined
            && this.minWeight == undefined
            && this.maxWeight == undefined;
    }

    getPlayers() {
        return this.players;
    }

    setPlayers(value) {
        this.players = value > 0 ? value : undefined;
    }

    getMinWeight() {
        return this.minWeight;
    }

    setMinWeight(value) {
        this.minWeight = value == 1.0 ? undefined : value;
    }

    getMaxWeight() {
        return this.maxWeight;
    }

    setWeightRange(min, max) {
        this.setMinWeight(min);
        this.setMaxWeight(max);
    }

    setMaxWeight(value) {
        this.maxWeight = value == 5.0 ? undefined : value;
    }
}

export default FilterData;
