import pytest
from processing.codelists import MarketEvaluationPointType
from processing.validation.rules.vr_250 import validate_vr_250


@pytest.mark.parametrize(
    "quantity,market_evaluation_point_type,expected",
    [
        pytest.param(
            1E6 - 1, MarketEvaluationPointType.production.value, True, id="valid because production limit is not exceeded"
        ),
        pytest.param(
            1E6, MarketEvaluationPointType.production.value, False, id="invalid because production limit is exceeded"
        ),
        pytest.param(
            1E6, MarketEvaluationPointType.consumption.value, True, id="valid when exceeding limit because it's not a production metering point"
        ),
    ],
)
def test_vr_250(quantity, market_evaluation_point_type, expected, enriched_data_factory):
    data = enriched_data_factory(quantity=quantity, market_evaluation_point_type=market_evaluation_point_type)
    validated_data = validate_vr_250(data)
    assert validated_data.first()["VR-250-Is-Valid"] == expected
