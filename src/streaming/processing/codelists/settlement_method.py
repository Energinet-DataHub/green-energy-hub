from enum import Enum

class SettlementMethod(Enum):
  flex_settled = "D01"
  profiled = "E01"
  non_profiled = "E02"
