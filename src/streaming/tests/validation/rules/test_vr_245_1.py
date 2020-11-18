import pytest
from geh_stream.codelists import MarketEvaluationPointType
from geh_stream.validation.rules.vr_245_1 import validate_vr_245_1


@pytest.mark.parametrize(
    "quantity,market_evaluation_point_type,expected",
    [
        pytest.param(
            0.0, MarketEvaluationPointType.consumption.value, True, id="valid because quantity is not negative"
        ),
        pytest.param(
            -1.0, MarketEvaluationPointType.consumption.value, False, id="invalid because quantity is negative"
        ),
        pytest.param(
            -1.0, MarketEvaluationPointType.production.value, True, id="valid when quantity is negative because it's not a consumption metering point"
        ),
    ],
)
def test_vr_245_1(quantity, market_evaluation_point_type, expected, enriched_data_factory):
    data = enriched_data_factory(quantity=quantity,
                                 market_evaluation_point_type=market_evaluation_point_type)
    validated_data = validate_vr_245_1(data)
    assert validated_data.first()["VR-245-1-Is-Valid"] == expected
