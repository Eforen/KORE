
*******************************
KS++ Specification Design Ideas
*******************************

I think it would be nice to be able to avoid running flag vars so we can code like this:
..code-block::
    :emphasize-lines: 6
    <T> find(dict: Dictionary<int, T>, target: T) {
        foreach i, value in dict {
            if value == target {
                break;
            }
        } nobreak {
            return -1;
        }
        return i;
    }

instead of something like this:
..code-block::
    :emphasize-lines: 2, 5, 9
    <T> find(dict: Dictionary<int, T>, target: T) {
        auto found = false;
        foreach i, value in dict {
            if value == target {
                found = True;
                break;
            }
        }
        if not found {
            return -1;
        }
        return i;
    }